import { Component, PLATFORM_ID, Inject } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { DocumentService } from '../document.service';

@Component({
  selector: 'app-md-to-docx',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatFormFieldModule,
    MatInputModule,
  ],
  templateUrl: './md-to-docx.html',
  styleUrls: ['./md-to-docx.css'],
})
export class MdToDocxComponent {
  markdownContent = '';
  fileName = 'document';
  selectedFile: File | null = null;
  converting = false;
  error: string | null = null;
  successMessage: string | null = null;
  activeTab: 'text' | 'file' = 'text';

  constructor(
    private documentService: DocumentService,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      const file = input.files[0];
      if (!file.name.endsWith('.md')) {
        this.error = 'Please select a valid .md (Markdown) file.';
        this.selectedFile = null;
        return;
      }
      this.selectedFile = file;
      this.fileName = file.name.replace(/\.md$/i, '');
      this.error = null;
    }
  }

  convertText(): void {
    if (!this.markdownContent.trim()) {
      this.error = 'Please enter some Markdown content.';
      return;
    }
    if (!isPlatformBrowser(this.platformId)) return;

    this.converting = true;
    this.error = null;
    this.successMessage = null;

    this.documentService.convertMarkdownToDocx(this.markdownContent, this.fileName).subscribe({
      next: (blob) => {
        this.downloadBlob(blob, `${this.fileName || 'document'}.docx`);
        this.successMessage = 'Document converted and downloaded successfully!';
        this.converting = false;
      },
      error: (err) => {
        this.error = err.error?.message ?? 'Conversion failed. Please try again.';
        this.converting = false;
      },
    });
  }

  convertFile(): void {
    if (!this.selectedFile) {
      this.error = 'Please select a .md file to convert.';
      return;
    }
    if (!isPlatformBrowser(this.platformId)) return;

    this.converting = true;
    this.error = null;
    this.successMessage = null;

    this.documentService.convertMarkdownFileToDocx(this.selectedFile).subscribe({
      next: (blob) => {
        this.downloadBlob(blob, `${this.fileName || 'document'}.docx`);
        this.successMessage = 'Document converted and downloaded successfully!';
        this.converting = false;
      },
      error: (err) => {
        this.error = err.error?.message ?? 'Conversion failed. Please try again.';
        this.converting = false;
      },
    });
  }

  private downloadBlob(blob: Blob, fileName: string): void {
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = fileName;
    a.click();
    URL.revokeObjectURL(url);
  }

  clearMessages(): void {
    this.error = null;
    this.successMessage = null;
  }
}
