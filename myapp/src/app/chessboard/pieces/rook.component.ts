import { CommonModule } from "@angular/common";
import { Component, Input } from "@angular/core";



@Component({
  standalone: true,
    imports: [CommonModule],
    selector: 'app-rook',
    templateUrl: './rook.component.html',
    styles: [`
    span {
        line-height: 70px;
        }
    `]
})
export class RookComponent {
    @Input() color!: string; 

}
