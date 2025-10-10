import { Component, Input, OnInit, SimpleChanges } from '@angular/core';
import { CreateGameRequest, CreateReplayRequest, GameResponse, GameService } from './game.service'; // adjust path as needed
import { AppComponent } from './app.component';
import { ContainerComponent } from './chessboard/container.component';
import { ActivatedRoute, Router} from '@angular/router'
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ReplayBoardComponent, ReplayContainerComponent } from './chessboard/replay.component';

const THEMES = {
        classic: { dark: '#769656', light: '#eeeed2' },
        red:     { dark: '#a33636ff', light: '#e0cbd8ff' },
        aqua:    { dark: '#315ff7ff', light: '#b0e1ffff'}
} as const;

type ThemeName = keyof typeof THEMES;
type Theme = { light: string; dark: string };

@Component({
    standalone: true,
  selector: 'replay-menu',
    imports: [FormsModule, CommonModule, ContainerComponent],
  templateUrl: './replaymenu.component.html'
})

export class ReplayMenuComponent implements OnInit{
  currentThemeName: ThemeName = 'classic';
  currentTheme: Theme = THEMES[this.currentThemeName];  
  activeUserId: number = 2;
  gameResponse: any;
  interval: any | undefined;
  gameId: number | null = null;
  turnCounter = 0;
  public snapshots: any[][] = [];
    public currentSnapshot: any;


  constructor(private chessService: GameService,
    private route: ActivatedRoute,
    private router: Router, 
    private game: GameService) {
  }

  ngOnInit(): void {
  this.route.paramMap.subscribe(params => {
    const idParam = params.get('id');
    const id = idParam ? +idParam : null;

    if (id !== null) {
      this.gameId = id;
      console.log('Loaded Game ID from route:', this.gameId);

      this.chessService.getReplayByGameId(this.gameId).subscribe({
        next: (data) => {
          this.snapshots = data;
          this.getReplay();
        },
        error: (err) => {
          console.error('Failed to load snapshots:', err);
        }
      });

      this.chessService.getColors(this.gameId).subscribe({
        next: (response: GameResponse) => {
          this.gameResponse = response;
          console.log('Loaded game details:', this.gameResponse);
        },
        error: (err) => {
          console.error('Failed loading game details:', err);
        }
      });
    } else {
      console.warn('Invalid game ID from route.');
    }
  });
}


    ngOnChanges(changes: SimpleChanges): void {
      if (changes['gameId'] && this.gameId) {
        clearInterval(this.interval);
        this.game.getReplayByGameId(this.gameId).subscribe(data => 
          {
            this.snapshots = data;
            this.turnCounter = 0;
            this.getReplay();
          }
        )}
    }
  
    getReplay(): void {
    this.interval = setInterval(() => {
        if (this.turnCounter >= this.snapshots.length) {
        clearInterval(this.interval);
        return;
        }
        this.currentSnapshot = this.snapshots[this.turnCounter];
        this.turnCounter++;
    }, 500); // you can adjust the speed
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


  startGame(gameId: number): void {
    const request: CreateReplayRequest = {
      gameId: gameId
    };

    this.chessService.startReplay(request).subscribe({
      next: (response: GameResponse) => {
        this.gameId = response.gameId;
        console.log('Game replaying with ID:', this.gameId);
        this.chessService.setGameId(this.gameId);
        this.router.navigate(['/replay/', gameId]);
      },
      error: (err) => {
        console.error('Failed to replay game:', err);
      }
    });
  }
}