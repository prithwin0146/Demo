import { Injectable } from '@angular/core';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private defaultConfig: MatSnackBarConfig = {
    duration: 3000,
    horizontalPosition: 'center',
    verticalPosition: 'bottom'
  };

  constructor(private snackBar: MatSnackBar) {}

  showSuccess(message: string): void {
    this.snackBar.open(message, 'Close', {
      ...this.defaultConfig,
      panelClass: ['success-snackbar']
    });
  }

  showError(message: string): void {
    this.snackBar.open(message, 'Close', {
      ...this.defaultConfig,
      duration: 5000,
      panelClass: ['error-snackbar']
    });
  }

  showInfo(message: string): void {
    this.snackBar.open(message, 'Close', {
      ...this.defaultConfig,
      panelClass: ['info-snackbar']
    });
  }
}
