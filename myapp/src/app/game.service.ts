import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { Coord } from './chessboard/coord';

@Injectable({
  providedIn: 'root'
})
export class GameService {
  private apiUrl = 'https://localhost:7107/api';
  private knightSubject = new BehaviorSubject<Coord>({ x: 0, y: 0 });
  private currentGameId: number | null = null;
  knightPosition$ = this.knightSubject.asObservable();

  constructor(private http: HttpClient) {}

  startNewGame(request: CreateGameRequest): Observable<GameResponse> {
    return this.http.post<GameResponse>(`${this.apiUrl}/Chess/create-game`, request);
  }

  setGameId(gameId: number): void {
    this.currentGameId = gameId;
    localStorage.setItem('gameId', gameId.toString());
  }

  getBoardByGameId(gameId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/Chess/GetBoard?gameId=${gameId}`);
  }


  getGameId(): number | null {
    if (this.currentGameId) return this.currentGameId;

    const stored = localStorage.getItem('gameId');
    return stored ? parseInt(stored, 10) : null;
  }
  
    getColors(gameId: number): Observable<GameResponse> {
    return this.http.get<GameResponse>(`${this.apiUrl}/Chess/game-details?gameId=${gameId}`);
  }

  movePiece(
    pieceType: number,
    from: Coord,
    to: Coord,
    userId: number,
    gameId: number,
    pieceColor: string,
    onDone: (success: boolean) => void
  ): void {
    const body = { pieceType, from, to, userId, gameId, pieceColor };
    this.http.post<{ validMove: boolean; message: string; newPosition: Coord }>(
      `${this.apiUrl}/Chess/MovePiece`, body
    ).subscribe({
      next: res => {
        if (res.validMove) {
          onDone(true);
        } else {
          console.log('Invalid move:', res.message);
          onDone(false);
        }
      },
      error: err => {
        console.error('API error moving piece:', err);
        onDone(false);
      }
    });
  }
}

export interface CreateGameRequest {
  player1Id: number;
  player2Id: number;
}

export interface GameResponse {
  gameId: number;
  whitePlayerId: number,
  blackPlayerId: number
}
