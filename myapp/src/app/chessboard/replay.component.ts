import { Component, OnInit, Input, OnChanges, SimpleChanges } from '@angular/core';
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
import { ReplayMenuComponent } from '../replaymenu.component';

@Component({
      standalone: true,
  selector: 'replay-board',
  imports: [SquareComponent, KnightComponent, CommonModule, PawnComponent, BishopComponent, RookComponent, QueenComponent, KingComponent],
  styleUrls: ['./chessboard.component.scss'],
  template: `
    <div class="chessboard">
      <div *ngFor="let y of [0,1,2,3,4,5,6,7]" class="row">
        <app-square
          *ngFor="let x of [0,1,2,3,4,5,6,7]"
          [black]="isBlack({ x, y })"
          [pieceColor]="board[y][x]?.pieceColor"
        >
          <app-knight
            *ngIf="board[y][x]?.pieceType === 1"
            [color]="board[y][x]?.pieceColor"
          ></app-knight>
          <app-pawn
            *ngIf="board[y][x]?.pieceType === 0"
            [color]="board[y][x]?.pieceColor"
          ></app-pawn>
          <app-bishop
            *ngIf="board[y][x]?.pieceType === 2"
            [color]="board[y][x]?.pieceColor"
          ></app-bishop>
          <app-rook
            *ngIf="board[y][x]?.pieceType === 3"
            [color]="board[y][x]?.pieceColor"
          ></app-rook>
          <app-queen
            *ngIf="board[y][x]?.pieceType === 4"
            [color]="board[y][x]?.pieceColor"
          ></app-queen>
          <app-king
            *ngIf="board[y][x]?.pieceType === 5"
            [color]="board[y][x]?.pieceColor"

          ></app-king>
        </app-square>
      </div>
    </div>
  `
})
export class ReplayBoardComponent implements OnInit, OnChanges {
  @Input() gameId!: number;
  @Input() activeUserId!: number;
  turnCounter = 0;
  board: any[][] = Array.from({ length: 8 }, () => Array(8).fill(null));
  selectedPiece$ = new BehaviorSubject<Coord | null>(null);
  interval: any | undefined;
  public snapshots: any[][] = [];
  @Input() snapshot: any[] = [];
  constructor(private game: GameService) {}

  ngOnInit(): void {
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['snapshot'] && this.snapshot) {
      this.loadBoard(this.snapshot);
    }
  }

  loadBoard(snapshot: any): void {
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

  isBlack({ x, y }: Coord): boolean {
    return (x + y) % 2 === 1;
  }
}

@Component({
  standalone: true,
  selector: 'replay-container',
  imports: [ReplayBoardComponent, CommonModule],
  template: `
    <div class="container">
      <replay-board
        [snapshot]="snapshot"
        [gameId]="gameId!"
        [activeUserId]="activeUserId">
      </replay-board>
    </div>
  `,
  styleUrls: ['./container.component.scss']
})
export class ReplayContainerComponent {
  @Input() gameId!: number | null;
  @Input() activeUserId!: number;
  @Input() snapshot!: any[];
}
