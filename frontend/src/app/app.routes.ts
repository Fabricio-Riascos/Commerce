import { Routes } from '@angular/router';

import { Process } from './pages/process/process';
import { Quarantine } from './pages/quarantine/quarantine';
import { Upload } from './pages/upload/upload';

export const routes: Routes = [
  { path: '', redirectTo: 'upload', pathMatch: 'full' },
  { path: 'upload', component: Upload, title: 'Carga de archivo' },
  { path: 'process', component: Process, title: 'Procesamiento' },
  { path: 'quarantine', component: Quarantine, title: 'Registros con errores' },
  { path: '**', redirectTo: 'upload' },
];
