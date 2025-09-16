import {Component, EventEmitter, Input, Output } from '@angular/core'
import { CommonModule } from '@angular/common'
import { KnightComponent } from './pieces/knight.component';
import { CdkDragDrop, DragDropModule, CdkDragEnter, CdkDragExit, CdkDropList } from '@angular/cdk/drag-drop';


@Component({
    imports: [CommonModule, DragDropModule],
    selector: 'app-square',
    templateUrl: `./square.component.html`,
    styleUrl: `./square.component.css`
})
export class SquareComponent {

    @Input() pos!: { x:number, y:number };
    @Input() x!: number;
    @Input() y!: number;
    @Input() black?: boolean;
    @Input() pieceColor!: string; // Color of the piece (White or Black)
    @Input() theme!: { light: string; dark: string };
    @Input() highlighted = false;
    @Input() connectedIds: string[] = [];
    @Input() connectedTo: (CdkDropList|string)[] = [];

    @Output() dropped = new EventEmitter<CdkDragDrop<any>>();
    @Output() entered = new EventEmitter<CdkDragEnter<any>>();
    @Output() exited  = new EventEmitter<CdkDragExit<any>>();

    
    getStyle() {
    const themes = {
        classic: { dark: '#769656', light: '#eeeed2' },
        red:     { dark: '#a33636ff', light: '#e0cbd8ff' },
        aqua:    { dark: '#210e9dff', light: '#b0e1ffff'}
    };
    let currentTheme = themes.classic;
  const isDark = this.black;
  const bgColor = isDark ? this.theme.dark : this.theme.light;

  const styles: any = {
    backgroundColor: bgColor,
  };
    if (this.highlighted) {
      styles.backgroundImage = 'linear-gradient(rgba(255, 215, 0, 0.45), rgba(255, 215, 0, 0.45))';
  } else {
      styles.backgroundImage = 'none';
    }
    
    if (this.pieceColor === 'black') {
      styles.color = 'black';
      if (isDark) {
        styles.textShadow = '1px 1px 0 white';
      }
    } else if (this.pieceColor === 'white') {
        styles.color = 'white';
    } else {
      styles.color = isDark ? 'white' : 'black';
    }

    return styles;
  }
}