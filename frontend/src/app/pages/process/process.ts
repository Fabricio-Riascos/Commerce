import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';

import { ProcessResult } from '../../models/commerce.models';
import { CommerceService } from '../../services/commerce.service';
import { getApiErrorMessage } from '../../utils/api-error';

/** Pantalla de procesamiento: elige la fecha y ejecuta la validación. */
@Component({
  selector: 'app-process',
  imports: [RouterLink],
  templateUrl: './process.html',
  styleUrl: './process.css',
})
export class Process {
  private readonly commerceService = inject(CommerceService);

  protected readonly processDate = signal('');
  protected readonly processing = signal(false);
  protected readonly result = signal<ProcessResult | null>(null);
  protected readonly errorMessage = signal<string | null>(null);

  protected readonly canProcess = computed(() => !!this.processDate() && !this.processing());

  protected onDateChange(event: Event): void {
    this.processDate.set((event.target as HTMLInputElement).value);
    this.result.set(null);
    this.errorMessage.set(null);
  }

  /** Llama a la API para procesar la fecha seleccionada. */
  protected process(): void {
    if (!this.canProcess()) {
      return;
    }

    this.processing.set(true);
    this.result.set(null);
    this.errorMessage.set(null);

    this.commerceService.process(this.processDate()).subscribe({
      next: (result) => {
        this.result.set(result);
        this.processing.set(false);
      },
      error: (error) => {
        this.errorMessage.set(getApiErrorMessage(error));
        this.processing.set(false);
      },
    });
  }
}
