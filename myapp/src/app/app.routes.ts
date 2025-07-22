import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { GameComponent } from './game.component'; // adjust path

export const routes: Routes = [
  { path: 'game', component: GameComponent },
  { path: 'game/:id', component: GameComponent }, // allows gameId in URL
  { path: '', redirectTo: '/game', pathMatch: 'full' }
];