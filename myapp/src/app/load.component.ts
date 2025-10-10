import { Component, OnInit } from '@angular/core';
import { CreateGameRequest, GameResponse, GameService } from './game.service'; // adjust path as needed
import { AppComponent } from './app.component';
import { ContainerComponent } from './chessboard/container.component';
import { ActivatedRoute, Router, RouterLink} from '@angular/router'
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SquareComponent } from './chessboard/square.component';
import { BoardComponent } from './chessboard/chessboard.component';
import { text } from 'stream/consumers';
import { Subject } from 'rxjs';
import { ok } from 'assert';

const THEMES = {
        classic: { dark: '#769656', light: '#eeeed2' },
        red:     { dark: '#a33636ff', light: '#e0cbd8ff' },
        aqua:    { dark: '#315ff7ff', light: '#b0e1ffff'}
} as const;

type ThemeName = keyof typeof THEMES;
type Theme = { light: string; dark: string };


@Component({
  standalone: true,
  selector: 'app-load',
    imports: [FormsModule, ContainerComponent, CommonModule, BoardComponent, RouterLink],
  templateUrl: './load.component.html'
})

export class LoadComponent implements OnInit{
  currentThemeName: ThemeName = 'classic';
  currentTheme: Theme = THEMES[this.currentThemeName];  
  activeUserId!: number;
  whitePlayerId!: number;
  blackPlayerId!: number;
  playerColor: 'White' | 'Black' = 'White';
  gameId!: number;
  gameResponse: any;
  public endGameMessage: string = '';
  isMyTurn: boolean = false;
  private destroy$ = new Subject<void>();
  public currentTurnValue: string | null = null;
  private norm = (s: string) => s.trim().toLowerCase();
  private dequote = (s: string) => s.replace(/^"|"$/g, '');


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
      next: (resp: GameResponse) => {
        this.gameResponse = resp;
        console.log('Loaded game details:', resp);

        if (this.activeUserId === resp.whitePlayerId) {
          this.playerColor = 'White';
        } else if (this.activeUserId === resp.blackPlayerId) {
          this.playerColor = 'Black';
        } else {
          console.warn('activeUserId not in this game; defaulting to White');
          this.playerColor = 'White';
        }
        console.log('White' + resp.whitePlayerId)
        this.whitePlayerId = resp.whitePlayerId;
        this.blackPlayerId = resp.blackPlayerId;
      },
      error: (err) => console.error('Failed loading game details:', err)
      });
    }
  });
}

  setActiveUser(id: number) {
    this.activeUserId = id;
    console.log('[GameComponent] Active user switched to:', this.activeUserId);
  }

  loadGame(gameId: number): void {
    console.log("Loaded game id" + gameId);
    try {
      this.router.navigateByUrl("/game/" + gameId);
    } catch (error) {
      console.log(Error.toString);
    }
  }

}