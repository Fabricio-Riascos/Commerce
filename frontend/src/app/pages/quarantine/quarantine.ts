import { DatePipe } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';

import { QuarantineRecord } from '../../models/commerce.models';
import { CommerceService } from '../../services/commerce.service';
import { getApiErrorMessage } from '../../utils/api-error';

/** Pantalla de errores: lista los registros en cuarentena con su motivo. */
@Component({
  selector: 'app-quarantine',
  imports: [DatePipe],
  templateUrl: './quarantine.html',
  styleUrl: './quarantine.css',
})
export class Quarantine implements OnInit {
  private readonly commerceService = inject(CommerceService);

  protected readonly records = signal<QuarantineRecord[]>([]);
  protected readonly loading = signal(false);
  protected readonly errorMessage = signal<string | null>(null);

  ngOnInit(): void {
    this.load();
  }

  /** Consulta los registros en cuarentena. */
  protected load(): void {
    this.loading.set(true);
    this.errorMessage.set(null);

    this.commerceService.getQuarantine().subscribe({
      next: (records) => {
        this.records.set(records);
        this.loading.set(false);
      },
      error: (error) => {
        this.errorMessage.set(getApiErrorMessage(error));
        this.loading.set(false);
      },
    });
  }
}
