import {Component, Input } from '@angular/core'
import { CommonModule } from '@angular/common'
import { KnightComponent } from './knight.component';


@Component({
    imports: [CommonModule],
    selector: 'app-square',
    template: `<div [ngStyle]="getStyle()">
              <ng-content></ng-content>
            </div>`,
    styles: [`:host, div {
        display: block;
        height: 100%;
        width: 100%;
        text-align: center;
    }
    `]
})
export class SquareComponent {
    @Input() black?: boolean;
    @Input() pieceColor!: string; // Color of the piece (White or Black)
    
    getStyle() {
      const squareColor = this.black ? 'black' : 'white';
      const styles: any = {
        backgroundColor: squareColor === 'black' ? '#769656' : '#eeeed2',  // Green for black squares, light beige for white squares
      };
    
      if (this.pieceColor === 'black') {
        styles.color = 'black';
        if (squareColor === 'black') {
          styles.textShadow = '1px 1px 0 white';  // Make black pieces stand out on black squares
        }
      } else if (this.pieceColor === 'white') {
        styles.color = 'white';  // Set the piece color to white
      } else {
        // Default fallback logic when pieceColor is not provided
        styles.color = squareColor === 'black' ? 'white' : 'black';
      }
    
      return styles;
    }
    
      
    }