import { Component, Input, Output, EventEmitter } from '@angular/core';
import { BoardComponent } from './chessboard.component';
type Theme = { light: string; dark: string };


@Component({
  standalone: true,
  selector: 'app-container',
  imports: [BoardComponent],
  template: `<div class="container"><app-board 
  [gameId]="gameId!" [activeUserId]="activeUserId!"
  [theme]="theme"
  (activeUserIdChange)="onActiveUserIdChange($event)"
  (gameEnded)="onGameEnded($event)" 
  ></app-board></div>`,
  styleUrls: ['./container.component.css']
})
export class ContainerComponent {
@Input() gameId!: number | null;
@Input() activeUserId!: number;
@Input() theme!: Theme;
@Output() gameEnded = new EventEmitter<string>();
@Output() activeUserIdChange = new EventEmitter<number>(); 

  onActiveUserIdChange(id: number) {
    this.activeUserId = id;           
    this.activeUserIdChange.emit(id); 
  }

  onGameEnded(message: string) {
    this.gameEnded.emit(message);
  }
}