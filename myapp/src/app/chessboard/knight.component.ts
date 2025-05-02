import { Component } from "@angular/core";
import { CdkDragEnd, CdkDragMove, CdkDragStart, DragDropModule } from '@angular/cdk/drag-drop';
import { BoardComponent } from "./chessboard.component";
import { GameService } from "../game.service";
import { Coord } from "./coord";
import { Observable } from "rxjs";

@Component({
    selector: 'app-knight',
    imports: [DragDropModule],
    template: '<div cdkDrag (cdkDragMoved)="onDragMoved($event)"><span>♘</span></div>'
    ,
    styles: [`
    span {
        font-weight: 400;
        font-size: 54px;
        line-height: 70px;
        }
    `]
})
export class KnightComponent {

    onDragStart(event: CdkDragStart){
        console.log('Drag started', event);
    }

    onDragEnd(event: CdkDragEnd){
        console.log('Drag end', event);
    }
    onDragMoved(event: CdkDragMove<any>) {
        const pointerX = event.pointerPosition.x;
        const pointerY = event.pointerPosition.y;
      }      

    calculatePosition(pos: Coord){
        
    }


}
