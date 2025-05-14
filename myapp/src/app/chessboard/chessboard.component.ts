import { Component, OnInit } from '@angular/core';
import { SquareComponent } from './square.component';
import { KnightComponent } from './knight.component';
import { CommonModule } from '@angular/common';
import { Coord } from './coord';
import { GameService } from '../game.service';
import { Observable, BehaviorSubject } from 'rxjs';
import { PawnComponent } from './pawn.component';
import { BishopComponent as BishopComponent } from "./bishop.component";
import { RookComponent} from './rook.component';
import { QueenComponent } from './queen.component';
import { KingComponent } from "./king.component ";

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
            <!-- <app-pawn
                *ngIf="board[y][x]?.pieceType === 0"
                (click)="handlePieceClick($event, { x, y })"
            ></app-pawn> -->
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

            <!-- add more pieces as needed -->
            </app-square>
        </div>
        </div>
    `
    })
export class BoardComponent implements OnInit {
    board: any[][] = [];
  sixtyFour = new Array(64).fill(0).map((_, i) => i);

  knightPosition$!: Observable<Coord>;
  selectedPiece$ = new BehaviorSubject<Coord | null>(null);

  constructor(private game: GameService) {}

  ngOnInit(): void {
    this.knightPosition$ = this.game.knightPosition$;
    this.game.loadKnightPosition();
    this.loadBoard()
  }


  loadBoard(): void {
    this.game.getBoard().subscribe(boardData => {
      // Create an 8x8 board filled with nulls
      const board: any[][] = Array.from({ length: 8 }, () => Array(8).fill(null));
  
      for (const piece of boardData) {
        const { x, y } = piece.position;
        // Ensure that piece color is correctly assigned
        piece.color = piece.color || (piece.isWhite ? 'White' : 'Black');  // Use a fallback if color is not set
        board[y][x] = piece;      
      }
  
      this.board = board;
    });
  }
  
  
  
  // Handle click on the square
  handleSquareClick(pos: Coord): void {
    console.log('Square click registered');
    const selected = this.selectedPiece$.getValue();
    console.log('Selected value:', selected);
  
    if (selected) {
      if (selected.x === pos.x && selected.y === pos.y) {
        
        console.log('Clicked the same square again, no move');
      } else {
        const piece = this.board[selected.y][selected.x];
        const type = piece?.pieceType;
        this.game.movePiece(type, selected, pos, (success: boolean) => {
            if (success) {
                this.loadBoard();
            }   
        });
      this.selectedPiece$.next(null); // Deselect after the move
            }
        }
    }   
  
  

  // Handle click on a piece to select it
  handlePieceClick(event: MouseEvent, pos: Coord): void {
    event.stopPropagation(); // Prevent the event from propagating to the square click handler

    const selected = this.selectedPiece$.getValue();
    if (selected && selected.x === pos.x && selected.y === pos.y) {
      console.log('Already selected');
      this.selectedPiece$.next(null); // Deselect piece
    } else {
      console.log('Piece selected');
      this.selectedPiece$.next(pos); // Set the selected piece's position
    }
  }

  // Convert index to x, y coordinates on the chessboard
  xy(i: number): Coord {
    return { x: i % 8, y: Math.floor(i / 8) };
  }

  // Determine if the square is black or white
  isBlack({ x, y }: Coord): boolean {
    return (x + y) % 2 === 1;
  }
}

@Component({
  standalone: true,
  selector: 'app-container',
  imports: [BoardComponent],
  template: `<div class="container"><app-board></app-board></div>`,
  styleUrls: ['./container.component.scss']
})
export class ContainerComponent {}
