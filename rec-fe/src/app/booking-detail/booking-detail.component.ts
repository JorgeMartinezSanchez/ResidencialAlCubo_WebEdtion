import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule, Router } from '@angular/router';
import { BookingService, BookingResponse } from './../services/booking/booking.service';
import { GuestService, GuestResponse } from './../services/guest/guest.service';
import { RoomService, Room } from './../services/room/room.service';

@Component({
  selector: 'app-booking-detail',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="detail-container" *ngIf="booking">
      <!-- Header -->
      <div class="detail-header">
        <button class="back-btn" routerLink="/bookings">
          ← Volver a Reservas
        </button>
        <div class="booking-status" [class]="booking.status">
          {{ booking.status | uppercase }}
        </div>
      </div>

      <!-- Main Content -->
      <div class="detail-content">
        <div class="booking-info">
          <h1>Reserva #{{ booking.id }}</h1>
          
          <!-- Room Info -->
          <div class="info-card">
            <h2>🏠 Información de la Habitación</h2>
            <div class="info-grid">
              <div class="info-item">
                <span class="label">Número:</span>
                <span class="value">Habitación {{ booking.roomNumber }}</span>
              </div>
              <div class="info-item">
                <span class="label">Tipo:</span>
                <span class="value">{{ booking.roomTypeName }}</span>
              </div>
              <div class="info-item">
                <span class="label">Capacidad:</span>
                <span class="value">{{ roomCapacity }} personas</span>
              </div>
              <div class="info-item">
                <span class="label">Precio por noche:</span>
                <span class="value">Bs. {{ roomPrice }}</span>
              </div>
            </div>
          </div>

          <!-- Dates Info -->
          <div class="info-card">
            <h2>📅 Fechas de Estadía</h2>
            <div class="info-grid">
              <div class="info-item">
                <span class="label">Check-in:</span>
                <span class="value highlight">{{ booking.startDate }}</span>
              </div>
              <div class="info-item">
                <span class="label">Check-out:</span>
                <span class="value highlight">{{ booking.endDate }}</span>
              </div>
              <div class="info-item">
                <span class="label">Noches:</span>
                <span class="value">{{ totalNights }} noches</span>
              </div>
            </div>
          </div>

          <!-- Guests Info -->
          <div class="info-card">
            <h2>👥 Huéspedes</h2>
            <div class="guests-list">
              <div class="guest-item" *ngFor="let guest of guests; let i = index">
                <div class="guest-number">{{ i + 1 }}</div>
                <div class="guest-info">
                  <div class="guest-name">
                    {{ guest.firstName }} {{ guest.secondName }} {{ guest.lastName }}
                  </div>
                  <div class="guest-details">
                    <span>📄 {{ guest.idCard }}</span>
                    <span>📞 {{ guest.phoneNumber }}</span>
                    <span>✉️ {{ guest.email }}</span>
                  </div>
                </div>
              </div>
              <div class="empty-guests" *ngIf="guests.length === 0">
                No hay huéspedes registrados
              </div>
            </div>
          </div>

          <!-- Payment Info -->
          <div class="info-card payment-card">
            <h2>💰 Información de Pago</h2>
            <div class="payment-details">
              <div class="payment-row">
                <span>Subtotal ({{ totalNights }} noches x Bs. {{ roomPrice }})</span>
                <span>Bs. {{ roomPrice * totalNights }}</span>
              </div>
              <div class="payment-row" *ngIf="lateCheckOutCharge > 0">
                <span>Cargo por Late Check-out</span>
                <span class="warning">+ Bs. {{ lateCheckOutCharge }}</span>
              </div>
              <div class="payment-row total">
                <span>Total</span>
                <span class="total-amount">Bs. {{ booking.total }}</span>
              </div>
            </div>
          </div>

          <!-- Actions -->
          <div class="action-buttons">
            <button 
              *ngIf="booking.status === 'pending'"
              class="btn-checkin" 
              (click)="checkIn()">
              🚪 Realizar Check-in
            </button>
            <button 
              *ngIf="booking.status === 'active'"
              class="btn-checkout" 
              (click)="checkOut()">
              🔑 Realizar Check-out
            </button>
            <button 
              *ngIf="booking.status === 'pending'"
              class="btn-cancel" 
              (click)="cancelBooking()">
              ❌ Cancelar Reserva
            </button>
            <button 
              class="btn-back" 
              routerLink="/bookings">
              📋 Ver todas las reservas
            </button>
          </div>
        </div>

        <!-- Timeline -->
        <div class="booking-timeline">
          <h3>Línea de Tiempo</h3>
          <div class="timeline">
            <div class="timeline-step" [class.completed]="booking.status !== 'pending'">
              <div class="timeline-icon">📝</div>
              <div class="timeline-content">
                <strong>Reserva Creada</strong>
                <small>Estado: Pending</small>
              </div>
            </div>
            <div class="timeline-step" [class.completed]="booking.status === 'active' || booking.status === 'finished'">
              <div class="timeline-icon">🚪</div>
              <div class="timeline-content">
                <strong>Check-in</strong>
                <small *ngIf="booking.status !== 'pending'">Realizado</small>
                <small *ngIf="booking.status === 'pending'">Pendiente</small>
              </div>
            </div>
            <div class="timeline-step" [class.completed]="booking.status === 'finished'">
              <div class="timeline-icon">🔑</div>
              <div class="timeline-content">
                <strong>Check-out</strong>
                <small *ngIf="booking.status === 'finished'">Realizado</small>
                <small *ngIf="booking.status !== 'finished'">Pendiente</small>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Loading State -->
    <div class="loading" *ngIf="!booking">
      <div class="spinner"></div>
      <p>Cargando detalles de la reserva...</p>
    </div>
  `,
  styles: [`
    .detail-container {
      max-width: 1200px;
      margin: 0 auto;
      padding: 2rem;
    }

    .detail-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 2rem;
    }

    .back-btn {
      background: none;
      border: none;
      color: #ff6b35;
      cursor: pointer;
      font-size: 1rem;
      padding: 0.5rem 1rem;
      border-radius: 8px;
      transition: all 0.3s ease;
    }

    .back-btn:hover {
      background: #fff3e0;
      transform: translateX(-5px);
    }

    .booking-status {
      display: inline-block;
      padding: 0.5rem 1rem;
      border-radius: 20px;
      font-size: 0.85rem;
      font-weight: 600;
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

    .detail-content {
      display: grid;
      grid-template-columns: 1fr 350px;
      gap: 2rem;
    }

    .booking-info {
      display: flex;
      flex-direction: column;
      gap: 1.5rem;
    }

    .info-card {
      background: white;
      border-radius: 12px;
      padding: 1.5rem;
      box-shadow: 0 2px 8px rgba(0,0,0,0.1);
    }

    .info-card h2 {
      color: #1a1a2e;
      font-size: 1.25rem;
      margin-bottom: 1rem;
      padding-bottom: 0.5rem;
      border-bottom: 2px solid #ff6b35;
    }

    .info-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: 1rem;
    }

    .info-item {
      display: flex;
      flex-direction: column;
      gap: 0.25rem;
    }

    .info-item .label {
      font-size: 0.85rem;
      color: #666;
    }

    .info-item .value {
      font-size: 1.1rem;
      font-weight: 600;
      color: #1a1a2e;
    }

    .info-item .value.highlight {
      color: #ff6b35;
    }

    .guests-list {
      display: flex;
      flex-direction: column;
      gap: 1rem;
    }

    .guest-item {
      display: flex;
      gap: 1rem;
      padding: 1rem;
      background: #f8f9fa;
      border-radius: 8px;
      transition: all 0.3s ease;
    }

    .guest-item:hover {
      background: #fff3e0;
    }

    .guest-number {
      width: 32px;
      height: 32px;
      background: #ff6b35;
      color: white;
      border-radius: 50%;
      display: flex;
      align-items: center;
      justify-content: center;
      font-weight: bold;
    }

    .guest-info {
      flex: 1;
    }

    .guest-name {
      font-weight: 600;
      color: #1a1a2e;
      margin-bottom: 0.25rem;
    }

    .guest-details {
      display: flex;
      gap: 1rem;
      font-size: 0.85rem;
      color: #666;
      flex-wrap: wrap;
    }

    .payment-card {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
    }

    .payment-card h2 {
      border-bottom-color: white;
    }

    .payment-details {
      display: flex;
      flex-direction: column;
      gap: 0.75rem;
    }

    .payment-row {
      display: flex;
      justify-content: space-between;
      padding: 0.5rem 0;
    }

    .payment-row.total {
      border-top: 2px solid rgba(255,255,255,0.3);
      margin-top: 0.5rem;
      padding-top: 1rem;
      font-size: 1.2rem;
      font-weight: bold;
    }

    .total-amount {
      font-size: 1.5rem;
      color: #ffd700;
    }

    .warning {
      color: #ffd700;
    }

    .action-buttons {
      display: flex;
      gap: 1rem;
      flex-wrap: wrap;
    }

    .btn-checkin, .btn-checkout, .btn-cancel, .btn-back {
      padding: 0.75rem 1.5rem;
      border: none;
      border-radius: 8px;
      cursor: pointer;
      font-weight: 600;
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

    .btn-back {
      background: #2196f3;
      color: white;
    }

    .booking-timeline {
      background: white;
      border-radius: 12px;
      padding: 1.5rem;
      box-shadow: 0 2px 8px rgba(0,0,0,0.1);
      height: fit-content;
    }

    .booking-timeline h3 {
      color: #1a1a2e;
      margin-bottom: 1.5rem;
    }

    .timeline {
      display: flex;
      flex-direction: column;
      gap: 1.5rem;
    }

    .timeline-step {
      display: flex;
      gap: 1rem;
      opacity: 0.5;
      transition: all 0.3s ease;
    }

    .timeline-step.completed {
      opacity: 1;
    }

    .timeline-icon {
      width: 40px;
      height: 40px;
      background: #f8f9fa;
      border-radius: 50%;
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 1.2rem;
    }

    .timeline-step.completed .timeline-icon {
      background: #ff6b35;
    }

    .timeline-content {
      flex: 1;
    }

    .timeline-content strong {
      display: block;
      color: #1a1a2e;
    }

    .timeline-content small {
      font-size: 0.8rem;
      color: #666;
    }

    .loading {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      min-height: 400px;
    }

    .spinner {
      width: 50px;
      height: 50px;
      border: 3px solid #f3f3f3;
      border-top: 3px solid #ff6b35;
      border-radius: 50%;
      animation: spin 1s linear infinite;
      margin-bottom: 1rem;
    }

    @keyframes spin {
      0% { transform: rotate(0deg); }
      100% { transform: rotate(360deg); }
    }

    @media (max-width: 768px) {
      .detail-container {
        padding: 1rem;
      }
      
      .detail-content {
        grid-template-columns: 1fr;
      }
      
      .guest-details {
        flex-direction: column;
        gap: 0.25rem;
      }
      
      .info-grid {
        grid-template-columns: 1fr;
      }
    }
  `]
})
export class BookingDetailComponent implements OnInit {
  booking: BookingResponse | null = null;
  guests: GuestResponse[] = [];
  roomPrice: number = 0;
  roomCapacity: number = 0;
  totalNights: number = 0;
  lateCheckOutCharge: number = 0;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private bookingService: BookingService,
    private guestService: GuestService,
    private roomService: RoomService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadBookingDetails(parseInt(id));
    }
  }

  loadBookingDetails(bookingId: number): void {
    // Primero obtener todas las reservas para encontrar la que necesitamos
    this.bookingService.getBookings().subscribe({
      next: (bookings) => {
        this.booking = bookings.find(b => b.id === bookingId) || null;
        if (this.booking) {
          this.calculateNights();
          this.loadGuests(bookingId);
          this.loadRoomDetails(this.booking.roomNumber);
          this.loadLateCheckOut(bookingId);
        }
      },
      error: (err) => {
        console.error('Error loading booking:', err);
      }
    });
  }

  loadGuests(bookingId: number): void {
    this.guestService.getGuestsFromBooking(bookingId).subscribe({
      next: (guests) => {
        this.guests = guests;
      },
      error: (err) => {
        console.error('Error loading guests:', err);
      }
    });
  }

  loadRoomDetails(roomNumber: string): void {
    this.roomService.getRoomByNumber(roomNumber).subscribe({
      next: (room) => {
        this.roomPrice = room.price;
        this.roomCapacity = room.capacity;
      },
      error: (err) => {
        console.error('Error loading room details:', err);
      }
    });
  }

  loadLateCheckOut(bookingId: number): void {
    // Implementar llamada al endpoint de late checkout si existe
    // Por ahora lo dejamos en 0
    this.lateCheckOutCharge = 0;
  }

  calculateNights(): void {
    if (this.booking) {
      const start = new Date(this.booking.startDate);
      const end = new Date(this.booking.endDate);
      this.totalNights = Math.ceil((end.getTime() - start.getTime()) / (1000 * 3600 * 24));
    }
  }

  checkIn(): void {
    if (this.booking && confirm('¿Confirmar check-in para esta reserva?')) {
      this.bookingService.checkIn(this.booking.id).subscribe({
        next: () => {
          alert('✅ Check-in realizado exitosamente');
          this.loadBookingDetails(this.booking!.id);
        },
        error: (err) => {
          alert('❌ Error al hacer check-in: ' + err.error);
        }
      });
    }
  }

  checkOut(): void {
    if (this.booking && confirm('¿Confirmar check-out para esta reserva?')) {
      this.bookingService.checkOut(this.booking.id).subscribe({
        next: () => {
          alert('✅ Check-out realizado exitosamente');
          this.loadBookingDetails(this.booking!.id);
        },
        error: (err) => {
          alert('❌ Error al hacer check-out: ' + err.error);
        }
      });
    }
  }

  cancelBooking(): void {
    if (this.booking && confirm('¿Estás seguro de cancelar esta reserva? Esta acción no se puede deshacer.')) {
      this.bookingService.cancelBooking(this.booking.id).subscribe({
        next: () => {
          alert('✅ Reserva cancelada exitosamente');
          this.router.navigate(['/bookings']);
        },
        error: (err) => {
          alert('❌ Error al cancelar: ' + err.error);
        }
      });
    }
  }
}