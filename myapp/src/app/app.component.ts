import { Component, NgModule, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { RouterModule } from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-root',
  imports: [RouterModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
  
})

export class AppComponent implements OnInit {
  title = 'Chess Game';
  message = 'aaaaaaaaaaaaaaaaaaaaaaaaa';

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.http.get<any>('https://localhost:7107/api/Chess')
      .subscribe(response => this.message = response.message);
  }
}


