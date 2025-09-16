import { CommonModule } from "@angular/common";
import { Component, Input } from "@angular/core";



@Component({
    imports: [CommonModule],
    selector: 'app-queen',
    templateUrl: './queen.component.html',
    styles: [`
    span {
        line-height: 70px;
        }
    `]
})
export class QueenComponent {
    @Input() color!: string; 

}
