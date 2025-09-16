import { CommonModule } from "@angular/common";
import { Component, Input } from "@angular/core";



@Component({
    imports: [CommonModule],
    selector: 'app-king',
    templateUrl: './king.component.html',
    styles: [`
    span {
        line-height: 70px;
        }
    `]
})
export class KingComponent {
    @Input() color!: string; 

}
