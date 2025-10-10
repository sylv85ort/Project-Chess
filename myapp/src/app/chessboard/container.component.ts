import { Component, Input, Output, EventEmitter } from '@angular/core';
import { BoardComponent } from './chessboard.component';
type Theme = { light: string; dark: string };


@Component({
  standalone: true,
  selector: 'app-container',
  imports: [BoardComponent],
  template: `
  <div class="container">
    <app-board
      [snapshot]="snapshot" 
      [gameId]="gameId!" [activeUserId]="activeUserId!"
      [theme]="theme"
      (activeUserIdChange)="onActiveUserIdChange($event)"
      (gameEnded)="onGameEnded($event)"
      (moveSucceeded)="moveSucceeded.emit()">
    </app-board>
  </div>`,
  styleUrls: ['./container.component.css']
})
export class ContainerComponent {
@Input() gameId!: number | null;
@Input() activeUserId!: number;
@Input() isMyTurn!: boolean;
@Input() currentTurn!: string | null;
@Input() theme!: Theme;
@Input() snapshot!: any[];
@Output() gameEnded = new EventEmitter<string>();
@Output() activeUserIdChange = new EventEmitter<number>(); 
@Output() moveSucceeded = new EventEmitter<void>();


  onActiveUserIdChange(id: number) {
    this.activeUserId = id;           
    this.activeUserIdChange.emit(id); 
  }

  onGameEnded(message: string) {
    this.gameEnded.emit(message);
  }
}