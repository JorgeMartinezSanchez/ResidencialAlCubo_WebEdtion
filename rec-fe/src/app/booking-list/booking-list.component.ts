import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { BookingService, BookingResponse } from './../services/booking/booking.service';

@Component({
  selector: 'app-bookings-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="bookings-container">
      <div class="header">
        <h1>Mis Reservas</h1>
        <button class="btn-primary" routerLink="/booking/create">
          + Nueva Reserva
        </button>
      </div>

      <div class="bookings-grid">
        <div class="booking-card" *ngFor="let booking of bookings">
          <div class="booking-status" [class]="booking.status">
            {{ booking.status | uppercase }}
          </div>
          <div class="booking-room">
            <span class="room-number">Habitación {{ booking.roomNumber }}</span>
            <span class="room-type">{{ booking.roomTypeName }}</span>
          </div>
          <div class="booking-dates">
            <div class="date">
              <span>📅 Check-in:</span>
              <strong>{{ booking.startDate }}</strong>
            </div>
            <div class="date">
              <span>📅 Check-out:</span>
              <strong>{{ booking.endDate }}</strong>
            </div>
          </div>
          <div class="booking-total">
            <span>💰 Total:</span>
            <strong>Bs. {{ booking.total }}</strong>
          </div>
          <div class="booking-actions">
            <button 
              *ngIf="booking.status === 'pending'"
              class="btn-checkin" 
              (click)="checkIn(booking.id)">
              🚪 Check In
            </button>
            <button 
              *ngIf="booking.status === 'active'"
              class="btn-checkout" 
              (click)="checkOut(booking.id)">
              🔑 Check Out
            </button>
            <button 
              *ngIf="booking.status === 'pending'"
              class="btn-cancel" 
              (click)="cancelBooking(booking.id)">
              ❌ Cancelar
            </button>
            <button 
              class="btn-detail" 
              routerLink="/bookings/{{ booking.id }}">
              👁️ Ver Detalle
            </button>
          </div>
        </div>

        <div class="empty-state" *ngIf="bookings.length === 0">
          <div class="empty-icon">📅</div>
          <h3>No tienes reservas</h3>
          <p>Comienza creando tu primera reserva</p>
          <button class="btn-primary" routerLink="/booking/create">
            Crear Reserva
          </button>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .bookings-container {
      max-width: 1200px;
      margin: 0 auto;
      padding: 2rem;
    }

    .header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 2rem;
    }

    .header h1 {
      color: #1a1a2e;
      font-size: 2rem;
    }

    .btn-primary {
      background: #ff6b35;
      color: white;
      border: none;
      padding: 0.75rem 1.5rem;
      border-radius: 8px;
      cursor: pointer;
      font-weight: 600;
      transition: all 0.3s ease;
    }

    .btn-primary:hover {
      background: #e55a2a;
      transform: translateY(-2px);
    }

    .bookings-grid {
      display: grid;
      gap: 1.5rem;
    }

    .booking-card {
      background: white;
      border-radius: 12px;
      padding: 1.5rem;
      box-shadow: 0 2px 8px rgba(0,0,0,0.1);
      transition: all 0.3s ease;
    }

    .booking-card:hover {
      transform: translateY(-2px);
      box-shadow: 0 4px 12px rgba(0,0,0,0.15);
    }

    .booking-status {
      display: inline-block;
      padding: 0.25rem 0.75rem;
      border-radius: 20px;
      font-size: 0.75rem;
      font-weight: 600;
      margin-bottom: 1rem;
    }

    .booking-status.pending {
      background: #fff3e0;
      color: #ff9800;
    }

    .booking-status.active {
      background: #e3f2fd;
      color: #2196f3;
    }

    .booking-status.finished {
      background: #e8f5e9;
      color: #4caf50;
    }

    .booking-status.cancelled {
      background: #ffebee;
      color: #f44336;
    }

    .booking-room {
      margin-bottom: 1rem;
    }

    .room-number {
      font-size: 1.25rem;
      font-weight: 600;
      color: #1a1a2e;
    }

    .room-type {
      display: inline-block;
      margin-left: 0.5rem;
      color: #666;
      font-size: 0.9rem;
    }

    .booking-dates {
      display: flex;
      gap: 2rem;
      margin-bottom: 1rem;
      padding: 1rem 0;
      border-top: 1px solid #eee;
      border-bottom: 1px solid #eee;
    }

    .date {
      display: flex;
      gap: 0.5rem;
    }

    .booking-total {
      display: flex;
      justify-content: space-between;
      margin-bottom: 1rem;
      font-size: 1.1rem;
    }

    .booking-actions {
      display: flex;
      gap: 1rem;
      flex-wrap: wrap;
    }

    .btn-checkin, .btn-checkout, .btn-cancel, .btn-detail {
      padding: 0.5rem 1rem;
      border: none;
      border-radius: 6px;
      cursor: pointer;
      font-weight: 500;
      transition: all 0.3s ease;
    }

    .btn-checkin {
      background: #4caf50;
      color: white;
    }

    .btn-checkout {
      background: #ff9800;
      color: white;
    }

    .btn-cancel {
      background: #f44336;
      color: white;
    }

    .btn-detail {
      background: #2196f3;
      color: white;
    }

    .empty-state {
      text-align: center;
      padding: 4rem;
      background: white;
      border-radius: 12px;
    }

    .empty-icon {
      font-size: 4rem;
      margin-bottom: 1rem;
    }

    @media (max-width: 768px) {
      .bookings-container {
        padding: 1rem;
      }

      .booking-dates {
        flex-direction: column;
        gap: 0.5rem;
      }
    }
  `]
})
export class BookingsListComponent implements OnInit {
  bookings: BookingResponse[] = [];

  constructor(private bookingService: BookingService) {}

  ngOnInit(): void {
    this.loadBookings();
  }

  loadBookings(): void {
    this.bookingService.getBookings().subscribe({
      next: (data) => {
        this.bookings = data;
      },
      error: (err) => {
        console.error('Error loading bookings:', err);
      }
    });
  }

  checkIn(id: number): void {
    if (confirm('¿Confirmar check-in?')) {
      this.bookingService.checkIn(id).subscribe({
        next: () => {
          alert('Check-in realizado exitosamente');
          this.loadBookings();
        },
        error: (err) => {
          alert('Error al hacer check-in: ' + err.error);
        }
      });
    }
  }

  checkOut(id: number): void {
    if (confirm('¿Confirmar check-out?')) {
      this.bookingService.checkOut(id).subscribe({
        next: () => {
          alert('Check-out realizado exitosamente');
          this.loadBookings();
        },
        error: (err) => {
          alert('Error al hacer check-out: ' + err.error);
        }
      });
    }
  }

  cancelBooking(id: number): void {
    if (confirm('¿Estás seguro de cancelar esta reserva?')) {
      this.bookingService.cancelBooking(id).subscribe({
        next: () => {
          alert('Reserva cancelada');
          this.loadBookings();
        },
        error: (err) => {
          alert('Error al cancelar: ' + err.error);
        }
      });
    }
  }
}