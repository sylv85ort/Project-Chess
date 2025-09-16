import { Component, Input } from "@angular/core";
import { CommonModule } from "@angular/common";




@Component({
    selector: 'app-pawn',
    imports: [CommonModule],
    templateUrl: './pawn.component.html',
    styles: [`
    span {
        line-height: 70px;
        }
    `]
})
export class PawnComponent {
    @Input() color!: string; 

}
