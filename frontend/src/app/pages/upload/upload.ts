import { Component, computed, ElementRef, inject, signal, viewChild } from '@angular/core';

import { CommerceRow, UploadResult } from '../../models/commerce.models';
import { CommerceService } from '../../services/commerce.service';
import { getApiErrorMessage } from '../../utils/api-error';
import { isValidCommerceFileName, parseCommerceCsv } from '../../utils/commerce-csv';

/** Pantalla de carga: selecciona el CSV, lo previsualiza y lo envía a la API. */
@Component({
  selector: 'app-upload',
  templateUrl: './upload.html',
  styleUrl: './upload.css',
})
export class Upload {
  private readonly commerceService = inject(CommerceService);
  private readonly fileInput = viewChild.required<ElementRef<HTMLInputElement>>('fileInput');

  protected readonly file = signal<File | null>(null);
  protected readonly rows = signal<CommerceRow[]>([]);
  protected readonly parseErrors = signal<string[]>([]);
  protected readonly uploading = signal(false);
  protected readonly result = signal<UploadResult | null>(null);
  protected readonly errorMessage = signal<string | null>(null);

  protected readonly canSubmit = computed(
    () =>
      !!this.file() && this.rows().length > 0 && this.parseErrors().length === 0 && !this.uploading(),
  );

  /** Lee el archivo seleccionado y arma la previsualización. */
  protected async onFileSelected(event: Event): Promise<void> {
    this.resetState();

    const selected = (event.target as HTMLInputElement).files?.[0];
    if (!selected) {
      return;
    }

    if (!isValidCommerceFileName(selected.name)) {
      this.errorMessage.set('El nombre del archivo debe tener el formato commerce_DDMMYYYY.csv.');
      return;
    }

    const { rows, errors } = parseCommerceCsv(await selected.text());
    if (rows.length === 0 && errors.length === 0) {
      this.errorMessage.set('El archivo no contiene registros.');
      return;
    }

    this.file.set(selected);
    this.rows.set(rows);
    this.parseErrors.set(errors);
  }

  /** Envía el archivo al backend. */
  protected submit(): void {
    const file = this.file();
    if (!file || !this.canSubmit()) {
      return;
    }

    this.uploading.set(true);
    this.errorMessage.set(null);

    this.commerceService.upload(file).subscribe({
      next: (result) => {
        this.result.set(result);
        this.uploading.set(false);
        this.clearSelection();
      },
      error: (error) => {
        this.errorMessage.set(getApiErrorMessage(error));
        this.uploading.set(false);
      },
    });
  }

  /** Descarta el archivo seleccionado. */
  protected cancel(): void {
    this.resetState();
    this.clearSelection();
  }

  private resetState(): void {
    this.result.set(null);
    this.errorMessage.set(null);
    this.parseErrors.set([]);
  }

  private clearSelection(): void {
    this.file.set(null);
    this.rows.set([]);
    this.fileInput().nativeElement.value = '';
  }
}
