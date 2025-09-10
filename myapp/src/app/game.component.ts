import { Component, OnInit } from '@angular/core';
import { CreateGameRequest, GameResponse, GameService } from './game.service'; // adjust path as needed
import { AppComponent } from './app.component';
import { ContainerComponent } from './chessboard/chessboard.component';
import { ActivatedRoute, Router} from '@angular/router'
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SquareComponent } from './chessboard/square.component';

const THEMES = {
        classic: { dark: '#769656', light: '#eeeed2' },
        red:     { dark: '#a33636ff', light: '#e0cbd8ff' },
        aqua:    { dark: '#315ff7ff', light: '#b0e1ffff'}
} as const;

type ThemeName = keyof typeof THEMES;
type Theme = { light: string; dark: string };


@Component({
  standalone: true,
  selector: 'app-game',
    imports: [FormsModule, ContainerComponent, CommonModule, SquareComponent],
  templateUrl: './game.component.html'
})

export class GameComponent implements OnInit{
  currentThemeName: ThemeName = 'classic';
  currentTheme: Theme = THEMES[this.currentThemeName];  
  activeUserId: number = 2;
  gameId: number | null = null;
  gameResponse: any;
  public endGameMessage: string = '';


  constructor(private chessService: GameService,
    private route: ActivatedRoute,
    private router: Router) {
  }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.gameId = +id;
        console.log('Loaded Game ID from route:', this.gameId);

        this.chessService.getBoardByGameId(this.gameId).subscribe({
          next: (board) => {
            console.log('Loaded board:', board);
          },
          error: (err) => {
            console.error('Failed to load board sorry:', err);
          }
        });

        this.chessService.getColors(this.gameId).subscribe({
          next: (response: GameResponse) => {
            this.gameResponse = response;
            console.log('Loaded games details:', this.gameResponse)
          },
          error: (err) => {
            console.error('Failed loading game details:', err);
          }
        });
      }
    });
  }

    setActiveUser(id: number) {
      this.activeUserId = id;
      console.log('[GameComponent] Active user switched to:', this.activeUserId);
    }
  
  changeTheme(){
if (this.currentThemeName === 'classic') {
  this.currentThemeName = 'red';
} else if (this.currentThemeName === 'red') {
  this.currentThemeName = 'aqua';
} else {
  this.currentThemeName = 'classic';
}    this.currentTheme = THEMES[this.currentThemeName];
  }

  startGame(player1Id: number, player2Id: number): void {
    const request: CreateGameRequest = {
      player1Id: player1Id,
      player2Id: player2Id
    };
        this.endGameMessage = "";

    this.chessService.startNewGame(request).subscribe({
      next: (response: GameResponse) => {
        this.gameId = response.gameId;
        console.log('Game started with ID:', this.gameId);
        this.chessService.setGameId(this.gameId);
        this.router.navigate(['/game', this.gameId]);
      },
      error: (err) => {
        console.error('Failed to create game:', err);
      }
    });
  }

  onGameEnded(message: string): void {
    console.log("CHECKMATE MESSAGE:" + message);
    this.endGameMessage = message;
  }
}