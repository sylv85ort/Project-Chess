import { Component, NgModule, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { KnightComponent } from './chessboard/knight.component';
import { SquareComponent } from './chessboard/square.component';
import { CommonModule } from '@angular/common';
import { ContainerComponent, BoardComponent } from './chessboard/chessboard.component';
import { HttpClient } from '@angular/common/http';
import { PawnComponent } from './chessboard/pawn.component';
import { CreateGameRequest, GameResponse, GameService } from './game.service';
import { GameComponent } from './game.component';
import { RouterModule } from '@angular/router';
import { ReplayMenuComponent } from './replaymenu.component';
import { ReplayBoardComponent } from './chessboard/replay.component';

@Component({
  standalone: true,
  selector: 'app-root',
  imports: [
    KnightComponent, SquareComponent, CommonModule,
    ContainerComponent,
    BoardComponent,PawnComponent, GameComponent, ReplayMenuComponent, RouterModule
],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
  
})

export class AppComponent implements OnInit {
  title = 'Chess Game';
  message = 'aaaaaaaaaaaaaaaaaaaaaaaaa';

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.http.get<any>('https://localhost:7107/api/Chess')
      .subscribe(response => this.message = response.message);
  }

  
}


