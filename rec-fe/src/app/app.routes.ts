import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { BookingCreateComponent } from './booking-create/booking-create.component';
import { BookingsListComponent } from './booking-list/booking-list.component'
import { RoomsListComponent } from './rooms-list/rooms-list.component';
import { BookingDetailComponent } from './booking-detail/booking-detail.component';

export const routes: Routes = [
  { 
    path: '', 
    component: HomeComponent,
    title: 'Residencial Al Cubo - Inicio'
  },
  { 
    path: 'booking/create', 
    component: BookingCreateComponent,
    title: 'Crear Reserva'
  },
  { 
    path: 'bookings', 
    component: BookingsListComponent,
    title: 'Mis Reservas'
  },
  { 
    path: 'bookings/:id', 
    component: BookingDetailComponent,
    title: 'Detalle de Reserva'
  },
  { 
    path: 'rooms', 
    component: RoomsListComponent,
    title: 'Nuestras Habitaciones'
  },
  { 
    path: '**', 
    redirectTo: '',
    pathMatch: 'full'
  }
];