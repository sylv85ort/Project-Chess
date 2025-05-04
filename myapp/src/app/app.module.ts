import { BrowserModule } from "@angular/platform-browser";
import { AppComponent } from "./app.component";
import { GameService } from "./game.service";
import { NgModule } from "@angular/core";
import { bootstrapApplication } from "@angular/platform-browser";
import { HttpClient, HttpErrorResponse } from "@angular/common/http";



@NgModule({
    imports: [BrowserModule, AppComponent, ],
    providers: [GameService, HttpClient]  })
  export class AppModule {}

  