import { Component, OnInit, PLATFORM_ID, Inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { AuthService } from '../../login/auth.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    RouterLinkActive,
    MatToolbarModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {
username: string = 'User';
role: string = 'Employee';

constructor(
  private authService: AuthService,
  private router: Router,
  @Inject(PLATFORM_ID) private platformId: Object
) {}

ngOnInit(): void {
  if (isPlatformBrowser(this.platformId)) {
    this.username = this.authService.getUsername() || 'User';
    this.role = this.authService.getUserRole() || 'Employee';
  }
}

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login'], { replaceUrl: true });
  }
}
