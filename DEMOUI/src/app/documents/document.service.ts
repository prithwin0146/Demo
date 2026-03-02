import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class DocumentService {
  private readonly baseUrl = 'http://localhost:5127/api/Document';

  constructor(private http: HttpClient) {}

  convertMarkdownToDocx(markdownContent: string, fileName: string): Observable<Blob> {
    return this.http.post(
      `${this.baseUrl}/convert`,
      { markdownContent, fileName },
      { responseType: 'blob' }
    );
  }

  convertMarkdownFileToDocx(file: File): Observable<Blob> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post(`${this.baseUrl}/convert-file`, formData, { responseType: 'blob' });
  }
}
