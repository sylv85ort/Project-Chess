import {Component} from '@angular/core'
import { SquareComponent } from './square.component';
import { KnightComponent } from './knight.component';
import { CommonModule } from '@angular/common';
import { Coord } from './coord';
import { GameService } from '../game.service';
import { Observable } from 'rxjs';
import { PawnComponent } from "./pawn.component";




@Component({
    standalone: true,
    selector: 'app-board',
    imports: [SquareComponent, KnightComponent, CommonModule, PawnComponent],
    styleUrls: ["./chessboard.component.scss"],
    template: `
    <div class="chessboard">
    <ng-container *ngIf="knightPosition$|async as kp">
    <div class="app-square" *ngFor="let i of sixtyFour">
                <app-square *ngIf="xy(i) as pos" [black]="isBlack(pos)" (click)="handleSquareClick(pos)">
                    <app-knight *ngIf="pos.x === kp.x && pos.y === kp.y"></app-knight>
                </app-square>
            </div>
        </ng-container>
    </div>
    `
})
export class BoardComponent {
    
    sixtyFour = new Array(64).fill(0).map((_, i) => i);
    public knightPosition$: Observable<Coord>;
    public pawnPosition$!: Observable<Coord>;
    public selectedPiece$!: Observable<Coord>;
    
    constructor(private game: GameService) {
    this.knightPosition$ = this.game.knightPosition$;
    this.knightPosition$.subscribe(pos => console.log("Knight6 position:", pos));
    }

    ngOnInit() {
        this.game.getKnightPosition().subscribe(position => {
          this.game.knightPosition$.next(position);
        });
        
        };

    
    handleSquareClick(pos: Coord) {   
        //if the selected square holds a particular piece {
        // this.selectedPiece$ = this.knightPosition$;
        this.game.moveKnight(pos);
        this.ngOnInit();
    }

    xy(i : number): Coord {
        return {
            x: i % 8,
            y: Math.floor(i / 8)
        }
      }
      isBlack({ x, y }: Coord) {
        return (x + y) % 2 === 1;
        
    
    }
}



@Component({
    standalone: true,
    selector: 'app-container',
    imports: [BoardComponent],
    template: `<div class="container"><app-board></app-board></div>`,
    styleUrls: ["./container.component.scss"]
  })
  export class ContainerComponent {

  }
  
  