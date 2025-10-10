import { Component, OnInit, Input, OnChanges, SimpleChanges, Output, EventEmitter, QueryList, ViewChildren } from '@angular/core';
import { SquareComponent } from './square.component';
import { KnightComponent } from './pieces/knight.component';
import { CommonModule } from '@angular/common';
import { Coord } from './coord';
import { GameService } from '../game.service';
import { Observable, BehaviorSubject } from 'rxjs';
import { PawnComponent } from './pieces/pawn.component';
import { BishopComponent } from "./pieces/bishop.component";
import { RookComponent } from './pieces/rook.component';
import { QueenComponent } from './pieces/queen.component';
import { KingComponent } from './pieces/king.component ';
import { Console, debug } from 'console';
import { DragDropModule, CdkDragDrop, moveItemInArray, transferArrayItem, CdkDropList, CdkDragEnter, CdkDragExit, CdkDrag, CdkDragHandle, CdkDragEnd } from '@angular/cdk/drag-drop';
import { exit } from 'process';
import e from 'express';
import { ActivatedRoute } from '@angular/router';
type Theme = { light: string; dark: string };

@Component({
  standalone: true,
  selector: 'app-board',
  imports: [SquareComponent, KnightComponent, CommonModule, PawnComponent, BishopComponent, RookComponent, QueenComponent, KingComponent, DragDropModule],
  styleUrl: './chessboard.styles.css',
  templateUrl: './chessboard.component.html'
})
export class BoardComponent implements OnInit, OnChanges {
  @ViewChildren(CdkDropList) private lists!: QueryList<CdkDropList>;
  allLists: CdkDropList[] = [];
  @Input() gameId!: number;
  @Input() activeUserId!: number;
  @Input() theme!: Theme;
  @Input() snapshot: any[] = [];


  @Output() gameEnded = new EventEmitter<string>();
  @Output() activeUserIdChange = new EventEmitter<number>();
  @Output() dropped = new EventEmitter<CdkDragDrop<any>>();
  @Output() entered = new EventEmitter<CdkDragEnter<Coord>>();
  @Output() exited  = new EventEmitter<CdkDragExit<Coord>>();
  @Output() moveSucceeded = new EventEmitter<void>();


  board: any[][] = Array.from({ length: 8 }, () => Array(8).fill(null));
  selectedPiece$ = new BehaviorSubject<Coord | null>(null);
  highlightSet = new Set<string>();
  endGameMessage: string = '';
  gameOver: boolean = false;
  currentPlayerColor: any;
  public snapshots: any[][] = [];
  public isReplayMode: boolean = false;


  squareIds: string[] = Array.from({ length: 8 }, (_, x) =>
    Array.from({ length: 8 }, (_, y) => `sq-${x}-${y}`)
  ).flat();

  constructor(private game: GameService) {}

  ngOnInit(): void {
    this.loadBoard();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['gameId'] && this.gameId) {
      this.loadBoard();
    }
    if (changes['activeUserId'] && this.activeUserId) {
      this.loadBoard();
    }
    if (changes['snapshot'] && this.snapshot) {
      this.loadReplay(this.snapshot);
    }
  }

  ngAfterViewInit() {
    this.allLists = this.lists.toArray();
    console.log('drop lists:', this.lists.length);
  }

  loadBoard(): void {
    this.isReplayMode==false;
    this.game.getBoardByGameId(this.gameId).subscribe(boardData => {
      const newBoard: any[][] = Array.from({ length: 8 }, () => Array(8).fill(null));
      for (const piece of boardData) {
        const { x, y } = piece.position;
        newBoard[y][x] = piece;
      }
      this.board = newBoard;
    });
  }

  loadReplay(snapshot: any): void {
  this.isReplayMode==true;
  this.board = this.createEmptyBoard();
    for (let piece of snapshot) {
        const x = piece.position.x;
        const y = piece.position.y;
        this.board[y][x] = {
          pieceType: piece.pieceType,
          pieceColor: piece.pieceColor
        }
    }
  }
    createEmptyBoard(): any[][] {
    const board = [];
    for (let i = 0; i < 8; i++) {
      board[i] = new Array(8).fill(null);
    }
    return board;
  }

  onDragStart(pos: Coord) {
    const piece = this.board[pos.x][pos.y]
    this.fetchAllLegalMoves(pos);
    console.log('drag start', pos);
  }

  onDragEnd(e: CdkDragEnd, from: Coord) {
    this.clearHighlights();

    const el = e.source.element.nativeElement as HTMLElement;
    const base = el.getBoundingClientRect();
    const offset = e.source.getFreeDragPosition();
    console.log(offset);

    const midX = base.left + offset.x + base.width  / 2;
    const midY = base.top  + offset.y + base.height / 2;

    const stack = document.elementsFromPoint(midX, midY) as HTMLElement[];
    const squareEl =
      stack.find(n => n?.hasAttribute?.('data-x') && n?.hasAttribute?.('data-y')) ||
      (document.elementFromPoint(midX, midY)?.closest('.square') as HTMLElement | null);

    if (!squareEl) { e.source.reset(); return; }

    const dx = squareEl.getAttribute('data-x');
    const dy = squareEl.getAttribute('data-y');
    if (dx == null || dy == null) { e.source.reset(); return; }

    const to: Coord = { x: parseInt(dx, 10), y: parseInt(dy, 10) };

    if (
      Number.isNaN(to.x) || Number.isNaN(to.y) ||
      to.x < 0 || to.x > 7 || to.y < 0 || to.y > 7
    ) { e.source.reset(); return; }

    if (to.x === from.x && to.y === from.y) { e.source.reset(); return; }

    this.attemptMove(from, to);
    e.source.reset();
  }

  handleSquareClick(pos: Coord): void {
    const selected = this.selectedPiece$.getValue();
    if (selected) {
      const piece = this.board[selected.y][selected.x];
    if (piece) {
      this.attemptMove(selected, pos);
      }  
    this.selectedPiece$.next(null);
    }
  }

  handlePieceClick(event: MouseEvent, pos: Coord): void {
  const piece = this.board[pos.y][pos.x];
  const selected = this.selectedPiece$.getValue();
  const isOwn = piece.pieceColor === this.currentPlayerColor; 
      this.fetchAllLegalMoves(pos);
  if (isOwn) {
    event.stopPropagation(); 
    this.selectedPiece$.next(pos);
    return;
  }

  if (selected) {
    event.stopPropagation();    
    this.handleSquareClick(pos);     
  } else {
  }
    event.stopPropagation();
    console.log("Piece clicked")
    this.selectedPiece$.next(pos);
  }

  attemptMove(from: Coord, to: Coord){
    const piece = this.board[from.y][from.x];
    this.game.movePiece(piece.pieceType, from, to, this.activeUserId, this.gameId, piece.pieceColor, (res) => {
    if (res.validMove) this.moveSucceeded.emit();
    {
      if (res.message == "Move successful"){
        this.game.switchActiveUserId(this.gameId, this.activeUserId).subscribe(r => {
          this.activeUserIdChange.emit(r.nextUserId);
        });
      }
      if (res.message === "Checkmate!") {
        this.game.declareGameResult(this.gameId, this.activeUserId).subscribe();
        this.gameEnded.emit(`Checkmate! Player ${this.activeUserId} wins!`);
      } else if (res.message === "Stalemate!") {
        this.game.declareGameResult(this.gameId, null).subscribe();
        this.gameEnded.emit("Stalemate!");
      }
      this.loadBoard();
      } if (res.validMove == false) {
      console.log("Move rejected:", res.message);
      alert("Invalid move: " + res.message);
      }
    this.selectedPiece$.next(null);
    this.clearHighlights();
    });
  } 

  fetchAllLegalMoves(pos: Coord) {
    this.game.fetchLegalMoves(this.gameId, pos).subscribe(moves => {
      this.highlightSet = new Set(moves.map(m => `${m.x}, ${m.y}`));
    });
  }

  clearHighlights() { 
    this.highlightSet = new Set(); 
  }

  isHighlighted(x: number, y: number) {
  return this.highlightSet.has(`${x}, ${y}`);
}

  isBlack({ x, y }: Coord): boolean {
    return (x + y) % 2 === 1;
  }
}
