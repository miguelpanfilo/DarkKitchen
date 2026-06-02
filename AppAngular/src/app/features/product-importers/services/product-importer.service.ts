import { Injectable, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import * as signalR from '@microsoft/signalr';
import { HttpErrorResponse } from '@angular/common/http';

export interface Importer {
  id: string;
  name: string;
}

@Injectable({ providedIn: 'root' })
export class ProductImporterService implements OnDestroy {
  private apiUrl = 'http://localhost:5254';
  private hubConnection!: signalR.HubConnection;
  private importers$ = new BehaviorSubject<Importer[]>([]);

  constructor(private http: HttpClient) {
    this.startConnection();
  }

  getImporters(): Observable<Importer[]> {
    return this.importers$.asObservable();
  }

  loadImporters(): void {
    this.http.get<Importer[]>(`${this.apiUrl}/api/v1/product-importers`)
      .subscribe(importers => this.importers$.next(importers));
  }

  importProducts(importerName: string, file: File): Observable<any> {
    const formData = new FormData();
    formData.append('importerName', importerName);
    formData.append('file', file);
    return this.http.post(`${this.apiUrl}/api/v1/product-imports`, formData);
  }

  private startConnection(): void {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${this.apiUrl}/hubs/product-importers`)
      .withAutomaticReconnect()
      .build();

    this.hubConnection.on('ImportersChanged', () => this.loadImporters());

    this.hubConnection.start()
      .then(() => this.loadImporters())
      .catch(err => {
        console.error('SignalR connection error:', err);
        this.loadImporters();
      });
  }

  ngOnDestroy(): void {
    this.hubConnection?.stop();
  }
}
