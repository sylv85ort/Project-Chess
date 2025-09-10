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
    @Input() theme!: { light: string; dark: string };
    
    getStyle() {
    const themes = {
        classic: { dark: '#769656', light: '#eeeed2' },
        red:     { dark: '#a33636ff', light: '#e0cbd8ff' },
        aqua: { dark: '#210e9dff', light: '#b0e1ffff'}
    };
    let currentTheme = themes.classic;
  const isDark = this.black; // your square logic
  const bgColor = isDark ? this.theme.dark : this.theme.light;

  const styles: any = {
    backgroundColor: bgColor,
  };
    
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