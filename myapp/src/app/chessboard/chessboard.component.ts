import { Component, OnInit, Input, OnChanges, SimpleChanges, Output, EventEmitter } from '@angular/core';
import { SquareComponent } from './square.component';
import { KnightComponent } from './knight.component';
import { CommonModule } from '@angular/common';
import { Coord } from './coord';
import { GameService } from '../game.service';
import { Observable, BehaviorSubject } from 'rxjs';
import { PawnComponent } from './pawn.component';
import { BishopComponent } from "./bishop.component";
import { RookComponent } from './rook.component';
import { QueenComponent } from './queen.component';
import { KingComponent } from './king.component ';

@Component({
  standalone: true,
  selector: 'app-board',
  imports: [SquareComponent, KnightComponent, CommonModule, PawnComponent, BishopComponent, RookComponent, QueenComponent, KingComponent],
  styleUrls: ['./chessboard.component.scss'],
  template: `
    <div class="chessboard">
      <div *ngFor="let y of [0,1,2,3,4,5,6,7]" class="row">
        <app-square
          *ngFor="let x of [0,1,2,3,4,5,6,7]"
          [black]="isBlack({ x, y })"
          [pieceColor]="board[y][x]?.pieceColor"
          (click)="handleSquareClick({ x, y })"
        >
          <app-knight
            *ngIf="board[y][x]?.pieceType === 1"
            [color]="board[y][x]?.pieceColor"
            (click)="handlePieceClick($event, { x, y })"
          ></app-knight>
          <app-pawn
            *ngIf="board[y][x]?.pieceType === 0"
            [color]="board[y][x]?.pieceColor"
            (click)="handlePieceClick($event, { x, y })"
          ></app-pawn>
          <app-bishop
            *ngIf="board[y][x]?.pieceType === 2"
            [color]="board[y][x]?.pieceColor"
            (click)="handlePieceClick($event, { x, y })"
          ></app-bishop>
          <app-rook
            *ngIf="board[y][x]?.pieceType === 3"
            [color]="board[y][x]?.pieceColor"
            (click)="handlePieceClick($event, { x, y })"
          ></app-rook>
          <app-queen
            *ngIf="board[y][x]?.pieceType === 4"
            [color]="board[y][x]?.pieceColor"
            (click)="handlePieceClick($event, { x, y })"
          ></app-queen>
          <app-king
            *ngIf="board[y][x]?.pieceType === 5"
            [color]="board[y][x]?.pieceColor"
            (click)="handlePieceClick($event, { x, y })"
          ></app-king>
        </app-square>
      </div>
    </div>
  `
})
export class BoardComponent implements OnInit, OnChanges {
  @Input() gameId!: number;
  @Input() activeUserId!: number;
  @Output() gameEnded = new EventEmitter<string>();
  board: any[][] = Array.from({ length: 8 }, () => Array(8).fill(null));
  selectedPiece$ = new BehaviorSubject<Coord | null>(null);
  endGameMessage: string = '';
  gameOver: boolean = false;

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
  }

  loadBoard(): void {
    this.game.getBoardByGameId(this.gameId).subscribe(boardData => {
      const newBoard: any[][] = Array.from({ length: 8 }, () => Array(8).fill(null));
      for (const piece of boardData) {
        const { x, y } = piece.position;
        newBoard[y][x] = piece;
      }
      this.board = newBoard;
    });
  }

  handleSquareClick(pos: Coord): void {
    const selected = this.selectedPiece$.getValue();
    if (selected) {
      const piece = this.board[selected.y][selected.x];
      if (piece) {
        this.game.movePiece(piece.pieceType, selected, pos, this.activeUserId, this.gameId, piece.pieceColor, (res) => {
  if (res.validMove) {
    if (res.message === "Checkmate!") {
      this.game.declareGameResult(this.gameId, this.activeUserId).subscribe();
      this.gameEnded.emit(`Checkmate! Player ${res.pieceColor} wins!`);
    } else if (res.message === "Stalemate!") {
      this.game.declareGameResult(this.gameId, null).subscribe();
      this.gameEnded.emit("Stalemate!");
    }

    this.loadBoard();
  } else {
    console.log("Move rejected:", res.message);
    alert("Invalid move: " + res.message);
  }
  this.selectedPiece$.next(null);
        });
      }
      
      this.selectedPiece$.next(null);

      
    }
  }

  handlePieceClick(event: MouseEvent, pos: Coord): void {
    event.stopPropagation();
    this.selectedPiece$.next(pos);
  }

  isBlack({ x, y }: Coord): boolean {
    return (x + y) % 2 === 1;
  }
}

@Component({
  standalone: true,
  selector: 'app-container',
  imports: [BoardComponent],
  template: `<div class="container"><app-board 
  [gameId]="gameId!" [activeUserId]="activeUserId!"
  (gameEnded)="onGameEnded($event)" 
  ></app-board></div>`,
  styleUrls: ['./container.component.scss']
})
export class ContainerComponent {
@Input() gameId!: number | null;
@Input() activeUserId!: number;
@Output() gameEnded = new EventEmitter<string>();
onGameEnded(message: string) {
  console.log("Container caught:", message); // TEMP DEBUG
  this.gameEnded.emit(message);
}
}