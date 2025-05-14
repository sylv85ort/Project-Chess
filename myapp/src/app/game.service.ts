import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Coord } from './chessboard/coord';

@Injectable({
  providedIn: 'root'
})
export class GameService {
  private knightSubject = new BehaviorSubject<Coord>({ x: 0, y: 0 });
  private pawnSubject = new BehaviorSubject<Coord>({ x: 0, y: 0 });
  private bishopSubject = new BehaviorSubject<Coord>({ x: 0, y: 0 });
  knightPosition$ = this.knightSubject.asObservable();

  constructor(private http: HttpClient) {}

  // Method to load knight's position
  loadKnightPosition() {
    this.http.get<Coord>('https://localhost:7107/api/Chess/GetKnightPosition')
      .subscribe({
        next: pos => {
          console.log('Knight position received:', pos);
          this.knightSubject.next(pos);
        },
        error: err => console.error('API error loading knight position:', err)
      });
  }

  getBoard(): Observable<any> {
    return this.http.get<any>('https://localhost:7107/api/Chess/GetBoard');
  }


  // General method to move any piece (like knight, queen, etc.)
  movePiece(pieceType: number, from: Coord, to: Coord, onDone: (success: boolean) => void): void {
    const body = { pieceType, from, to };
    console.log('Sending payload:', body); //
    this.http.post<{ validMove: boolean, message: string, newPosition: Coord }>(
      'https://localhost:7107/api/Chess/MovePiece', body
    ).subscribe({
      next: res => {
        if (res.validMove) {
          // Update the position of the piece, like knight, based on the move
          if (pieceType == 0) {
            onDone(true);
            //this.loadBoard();  // refreshes the board
          } else if (pieceType == 1) {
            onDone(true);
            //this.loadBoard();  // refreshes the board
          } else if (pieceType == 2) {
            onDone(true);
          } else if (pieceType == 3) {
            onDone(true);
          } else if (pieceType == 4) {
            onDone(true);
          } else if (pieceType == 5) {
            onDone(true);
          }
          // You can add more logic here for other pieces (like Queen, Bishop, etc.)
        } else {
          console.log('Invalid move:', res.message);
          onDone(false);
        }
      },
      error: err => {console.error('API error moving piece:', err)
        onDone(false);
        }
    });
  }
  

  // Specialized method to move knight
//   moveKnight(pieceType: String, from: Coord, to: Coord): void {
//     const body = { pieceType, from, to };
//     this.movePiece('Knight', from, to);  // Reuse movePiece for the knight
//     this.http.post<{ validMove: boolean, message: string, newPosition: Coord }>(
//         'https://localhost:7107/api/Chess/MovePiece', body
//       ).subscribe({
//         next: res => {
//           if (res.validMove) {
//             // Update the position of the piece, like knight, based on the move
//             if (pieceType === 'Knight') {
//               this.knightSubject.next(res.newPosition);
//             }
//             // You can add more logic here for other pieces (like Queen, Bishop, etc.)
//           } else {
//             console.log('Invalid move:', res.message);
//           }
//         },
//         error: err => console.error('API error moving piece:', err)
//       });  
//   }


    private mapPieceTypeToEnum(pieceType: string): number {
        switch (pieceType) {
        case 'Knight': return 1;
        case 'Pawn': return 0;
        default: return -1;  // Invalid piece
        }
    }
}

// enum PieceColor {
//     Black = 'black',
//     White = 'white'
//   }