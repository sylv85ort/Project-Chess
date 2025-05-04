import { Component, NgModule, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { KnightComponent } from './chessboard/knight.component';
import { SquareComponent } from './chessboard/square.component';
import { CommonModule } from '@angular/common';
import { ContainerComponent, BoardComponent } from './chessboard/chessboard.component';
import { HttpClient } from '@angular/common/http';
import { PawnComponent } from './chessboard/pawn.component';


@Component({
  selector: 'app-root',
  imports: [
    KnightComponent, SquareComponent, CommonModule,
    ContainerComponent,
    BoardComponent,PawnComponent
],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
  
})

export class AppComponent implements OnInit {
  title = 'myapp';
  message = 'aaaaaaaaaaaaaaaaaaaaaaaaa';

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.http.get<any>('https://localhost:7107/api/Chess')
      .subscribe(response => this.message = response.message);
  }
}




