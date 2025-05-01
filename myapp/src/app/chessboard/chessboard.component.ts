import {Component} from '@angular/core'
import { SquareComponent } from './square.component';
import { KnightComponent } from './knight.component';
import { CommonModule } from '@angular/common';
import { Coord } from './coord';
import { GameService } from '../game.service';
import { Observable } from 'rxjs';




@Component({
    standalone: true,
    selector: 'app-board',
    imports: [SquareComponent, KnightComponent, CommonModule ],
    styleUrls: ["./chessboard.component.scss"],
    template: `
    <div class="chessboard">
    <ng-container *ngIf="knightPosition$|async as kp">
    <div class="app-square" *ngFor="let i of sixtyFour">
                <app-square *ngIf="xy(i) as pos" [black]="isBlack(pos)" (click)="handleSquareClick(pos)">
                <app-knight *ngIf="pos.x === 0 && pos.y === 0"></app-knight>

                    <app-knight *ngIf="pos.x === kp.x && pos.y === kp.y"></app-knight>
                </app-square>
            </div>
        </ng-container>
    </div>
    `
})
export class BoardComponent {
    sixtyFour = new Array(64).fill(0).map((_, i) => i);
    public knightPosition$!: Observable<Coord>; 
    
    constructor(private game: GameService) {
        
        this.knightPosition$ = this.game.knightPosition$;
this.knightPosition$.subscribe(pos => console.log("Knight position:", pos));

    }
    
    handleSquareClick(pos: Coord) {
        
        if (this.game.canMoveKnight(pos)) {
            this.game.moveKnight(pos);
        }
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
  
  