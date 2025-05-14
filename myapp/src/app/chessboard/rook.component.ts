import { CommonModule } from "@angular/common";
import { Component, Input } from "@angular/core";



@Component({
    imports: [CommonModule],
    selector: 'app-rook',
    template: `
    <div>
      <span [ngStyle]="{ color: color === 'White' ? 'white' : 'black' }">
      ♔
      </span>
    </div>
  `,
    styles: [`
    span {
        font-weight: 400;
        font-size: 54px;
        line-height: 70px;
        }
    `]
})
export class RookComponent {
    @Input() color!: string; 

}
