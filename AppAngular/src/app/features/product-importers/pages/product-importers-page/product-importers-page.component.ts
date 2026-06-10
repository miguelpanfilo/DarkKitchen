import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProductImporterService, Importer } from '../../services/product-importer.service';
import { Observable, Subscription } from 'rxjs';
import { environment } from '../../../../../environments/environment';

@Component({
  selector: 'app-product-importers-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './product-importers-page.component.html',
  styleUrl: './product-importers-page.component.css'
})
export class ProductImportersPageComponent implements OnInit, OnDestroy {
  importers$!: Observable<Importer[]>;
  selectedImporterName: string = '';
  selectedFile: File | null = null;
  successMessage: string = '';
  errorMessage: string = '';
  loading: boolean = false;
  importedProducts: any[] = [];
  readonly apiUrl = environment.webApiBaseUrl;

  private subs = new Subscription();

  constructor(private importerService: ProductImporterService) {}

  ngOnDestroy(): void {
    this.subs.unsubscribe();
  }

  ngOnInit(): void {
    this.importers$ = this.importerService.getImporters();
  }

  onImporterChange(): void {
    this.import();
  }

  onFileChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.selectedFile = input.files?.[0] ?? null;
    this.import();
  }

  onFileDrop(event: DragEvent): void {
    event.preventDefault();
    this.selectedFile = event.dataTransfer?.files?.[0] ?? null;
    this.import();
  }

  onDragOver(event: DragEvent): void {
    event.preventDefault();
  }

  import(): void {
    if (!this.selectedImporterName || !this.selectedFile) return;

    this.loading = true;
    this.successMessage = '';
    this.errorMessage = '';

    this.subs.add(this.importerService.importProducts(this.selectedImporterName, this.selectedFile)
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
      }));
  }
}
