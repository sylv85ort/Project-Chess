import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { KnightComponent } from './chessboard/knight.component';
import { SquareComponent } from './chessboard/square.component';
import { CommonModule } from '@angular/common';
import { ContainerComponent, BoardComponent } from './chessboard/chessboard.component';



@Component({
  selector: 'app-root',
  imports: [
    KnightComponent, SquareComponent, CommonModule,
    ContainerComponent,
    BoardComponent
],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
  
})
export class AppComponent {
  title = 'myapp';
}


