import { Component, OnInit } from '@angular/core';
import { CreateGameRequest, GameResponse, GameService } from './game.service'; // adjust path as needed
import { AppComponent } from './app.component';
import { ContainerComponent } from './chessboard/chessboard.component';
import { ActivatedRoute, Router} from '@angular/router'
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  standalone: true,
  selector: 'app-game',
    imports: [FormsModule, ContainerComponent, CommonModule],
  templateUrl: './game.component.html'
})
export class GameComponent implements OnInit{
  activeUserId: number = 2;
  gameId: number | null = null;
  gameResponse: any;

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


  startGame(player1Id: number, player2Id: number): void {
    const request: CreateGameRequest = {
      player1Id: player1Id,
      player2Id: player2Id
    };

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
}