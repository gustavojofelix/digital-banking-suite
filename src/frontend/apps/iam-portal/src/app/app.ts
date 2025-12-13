import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
//import { ShellLayout } from './layout/shell-layout';

@Component({
  imports: [RouterModule],
  standalone: true,
  selector: 'app-root',
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  protected title = 'iam-portal';
}
