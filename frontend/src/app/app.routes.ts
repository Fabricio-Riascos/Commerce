import { Routes } from '@angular/router';

import { Process } from './pages/process/process';
import { Quarantine } from './pages/quarantine/quarantine';
import { Records } from './pages/records/records';
import { Upload } from './pages/upload/upload';

export const routes: Routes = [
  { path: '', redirectTo: 'upload', pathMatch: 'full' },
  { path: 'upload', component: Upload, title: 'Carga de archivo' },
  { path: 'process', component: Process, title: 'Procesamiento' },
  { path: 'records', component: Records, title: 'Registros cargados' },
  { path: 'quarantine', component: Quarantine, title: 'Registros con errores' },
  { path: '**', redirectTo: 'upload' },
];
