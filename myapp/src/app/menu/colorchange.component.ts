import {Component, Input} from '@angular/core';
import {MatIconModule} from '@angular/material/icon';
import {MatDividerModule} from '@angular/material/divider';
import {MatButtonModule} from '@angular/material/button';
import { SquareComponent } from '../chessboard/square.component';

/**
 * @title Basic buttons
 */
@Component({
  selector: 'button',
  templateUrl: 'button.html',
  styleUrl: 'button.css',
  imports: [MatButtonModule, MatDividerModule, MatIconModule, SquareComponent],
})
export class ButtonOverviewExample {}