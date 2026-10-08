import { Routes } from '@angular/router';

import { Upload } from './pages/upload/upload';

export const routes: Routes = [
  { path: '', redirectTo: 'upload', pathMatch: 'full' },
  { path: 'upload', component: Upload, title: 'Carga de archivo' },
  { path: '**', redirectTo: 'upload' },
];
