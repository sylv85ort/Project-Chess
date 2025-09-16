import { CommonModule } from "@angular/common";
import { Component, Input } from "@angular/core";



@Component({
  standalone: true,
    imports: [CommonModule],
    selector: 'app-bishop',
    templateUrl: './bishop.component.html',
    styles: [`
    span {
        line-height: 70px;
        }
    `]
})
export class BishopComponent {
    @Input() color!: string; 

}
