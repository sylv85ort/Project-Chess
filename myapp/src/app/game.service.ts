import { inject, Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { Coord } from './chessboard/coord';
import { HttpClient } from '@angular/common/http';
import { environment } from './environment';
import { debug } from 'node:console';

@Injectable({
    providedIn: 'root'
})

export class GameService {

    knightPosition$ = new BehaviorSubject<Coord>({ x: 2, y: 5 });
    constructor(private http: HttpClient) {

    }
    // knightPosition$ = new BehaviorSubject<Coord>({ x: 2, y: 5 });
    

    // constructor() {
    //     this.knightPosition$.subscribe(kp => {
    //         this.currentPosition = kp;
    //     })
    // }
    
    moveKnight(to: Coord) {
        this.http.post<any>('https://localhost:7107/api/Chess/MoveKnight', to)
            .subscribe({
                next: (response) => {
                    if (response.validMove) {
                        console.log('Knight new position:', response.newPosition);
                        this.knightPosition$.next(response.newPosition); // Use next() on the BehaviorSubject to update the position
                    }
                },
                error: (error) => {
                    console.error('Invalid gmove bitch', error);
                }
            });
    }
    
      

    getKnightPosition(): Observable<Coord> {
        return this.http.get<Coord>('https://localhost:7107/api/Chess/KnightPosition');
    }

    

    private apiUrl = environment.apiURL + '/Chess'

    public get(): Observable<any> {
        return this.http.get(this.apiUrl);
    }

    

}