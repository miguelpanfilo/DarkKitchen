import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProductImporterService, Importer } from '../../services/product-importer.service';
import { Observable } from 'rxjs';
import { SubmitButtonComponent } from '../../../../shared/components/submit-button/submit-button.component';
import { StatusMessagesComponent } from '../../../../shared/components/status-messages/status-messages.component';
import { CancelButtonComponent } from '../../../../shared/components/cancel-button/cancel-button.component';
import { environment } from '../../../../../environments/environment';
import { Router } from '@angular/router';

@Component({
  selector: 'app-product-importers-page',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    SubmitButtonComponent,
    CancelButtonComponent,
    StatusMessagesComponent
  ],
  templateUrl: './product-importers-page.component.html',
  styleUrl: './product-importers-page.component.css'
})
export class ProductImportersPageComponent implements OnInit {
  importers$!: Observable<Importer[]>;
  selectedImporterName: string = '';
  selectedFile: File | null = null;
  successMessage: string = '';
  errorMessage: string = '';
  loading: boolean = false;
  importedProducts: any[] = [];
  readonly apiUrl = environment.webApiBaseUrl;

 constructor(
    private importerService: ProductImporterService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.importers$ = this.importerService.getImporters();
  }

  onBack(): void {
    this.router.navigate(['/home/products']);
  }

  onImporterChange(): void {
  this.successMessage = '';
  this.errorMessage = '';
}

  onFileChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.selectedFile = input.files?.[0] ?? null;
  }

  onFileDrop(event: DragEvent): void {
    event.preventDefault();
    this.selectedFile = event.dataTransfer?.files?.[0] ?? null;
  }

  onDragOver(event: DragEvent): void {
    event.preventDefault();
  }

  onCancel(fileInput: HTMLInputElement): void {
    this.selectedFile = null;
    fileInput.value = '';

    this.successMessage = '';
    this.errorMessage = '';
    this.importedProducts = [];
  }

  import(): void {
    if (!this.selectedImporterName || !this.selectedFile) return;

    this.loading = true;
    this.successMessage = '';
    this.errorMessage = '';

    this.importerService.importProducts(this.selectedImporterName, this.selectedFile)
      .subscribe({
        next: (result: any[]) => {
          this.successMessage = `Importación exitosa: ${result.length} productos importados.`;
          this.importedProducts = result;
          this.loading = false;
        },
        error: (err) => {
          this.errorMessage = err.error?.message ?? 'Error al importar el archivo.';
          this.loading = false;
        }
      });
  }
}
