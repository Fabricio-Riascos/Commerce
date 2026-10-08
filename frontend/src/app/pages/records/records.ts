import { Component, inject, OnInit, signal } from '@angular/core';

import { CommerceRecord } from '../../models/commerce.models';
import { CommerceService } from '../../services/commerce.service';
import { Paginator } from '../../shared/paginator/paginator';
import { getApiErrorMessage } from '../../utils/api-error';

const PAGE_SIZE = 10;

/** Pantalla de registros: lista paginada de la tabla commerce con filtro opcional por fecha. */
@Component({
  selector: 'app-records',
  imports: [Paginator],
  templateUrl: './records.html',
  styleUrl: './records.css',
})
export class Records implements OnInit {
  private readonly commerceService = inject(CommerceService);

  protected readonly pageSize = PAGE_SIZE;
  protected readonly records = signal<CommerceRecord[]>([]);
  protected readonly processDate = signal('');
  protected readonly page = signal(1);
  protected readonly totalCount = signal(0);
  protected readonly loading = signal(false);
  protected readonly errorMessage = signal<string | null>(null);

  ngOnInit(): void {
    this.load();
  }

  /** Al cambiar el filtro se vuelve a la primera página. */
  protected onDateChange(event: Event): void {
    this.processDate.set((event.target as HTMLInputElement).value);
    this.load(1);
  }

  protected clearFilter(): void {
    this.processDate.set('');
    this.load(1);
  }

  /** Consulta la página indicada (por defecto, la actual). */
  protected load(page = this.page()): void {
    this.loading.set(true);
    this.errorMessage.set(null);

    this.commerceService.getCommerce(page, PAGE_SIZE, this.processDate()).subscribe({
      next: (result) => {
        this.records.set(result.items);
        this.page.set(result.page);
        this.totalCount.set(result.totalCount);
        this.loading.set(false);
      },
      error: (error) => {
        this.errorMessage.set(getApiErrorMessage(error));
        this.loading.set(false);
      },
    });
  }
}
