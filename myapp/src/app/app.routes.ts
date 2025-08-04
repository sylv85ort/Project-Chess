import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { GameComponent } from './game.component'; // adjust path
import { ReplayBoardComponent } from './chessboard/replay.component';
import { ReplayMenuComponent } from './replaymenu.component';

export const routes: Routes = [
  { path: 'game', component: GameComponent },
  { path: 'game/:id', component: GameComponent }, // allows gameId in URL
  { path: '', redirectTo: '/game', pathMatch: 'full' },
  { path: 'replay', component: ReplayMenuComponent },
  { path: 'replay/:id', component: ReplayMenuComponent },
];