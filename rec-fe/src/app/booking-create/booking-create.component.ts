import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { BookingService, Room, Guest } from './../services/booking/booking.service';
import { GuestService } from './../services/guest/guest.service';
import { RoomService } from './../services/room/room.service';

@Component({
  selector: 'app-booking-create',
  templateUrl: './booking-create.component.html',
  styleUrls: ['./booking-create.component.css']
})
export class BookingCreateComponent implements OnInit {
  bookingForm: FormGroup;
  rooms: Room[] = [];
  guests: Guest[] = [];
  filteredGuests: Guest[] = [];
  selectedGuests: Guest[] = [];
  totalPrice: number = 0;
  nights: number = 0;

  constructor(
    private fb: FormBuilder,
    private bookingService: BookingService,
    private guestService: GuestService,
    private roomService: RoomService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.bookingForm = this.fb.group({
      roomId: ['', Validators.required],
      startDate: ['', Validators.required],
      endDate: ['', Validators.required],
      guestSearch: [''],
      total: [{ value: 0, disabled: true }]
    });
  }

  ngOnInit(): void {
    this.loadRooms();
    this.loadGuests();
    
    // Escuchar cambios de fecha para recalcular total
    this.bookingForm.get('startDate')?.valueChanges.subscribe(() => this.calculateTotal());
    this.bookingForm.get('endDate')?.valueChanges.subscribe(() => this.calculateTotal());
    this.bookingForm.get('roomId')?.valueChanges.subscribe(() => this.calculateTotal());
    
    // Filtrar huéspedes
    this.bookingForm.get('guestSearch')?.valueChanges.subscribe(search => {
      this.filteredGuests = this.guests.filter(g => 
        g.firstName.toLowerCase().includes(search.toLowerCase()) ||
        g.lastName.toLowerCase().includes(search.toLowerCase()) ||
        g.idCard.includes(search)
      );
    });
  }

  loadRooms(): void {
    this.roomService.getAllRooms().subscribe({
      next: (rooms) => {
        this.rooms = rooms.filter(r => !r.occupied);
      },
      error: (err) => console.error('Error loading rooms:', err)
    });
  }

  loadGuests(): void {
    this.bookingService.getGuests().subscribe({
      next: (guests) => {
        this.guests = guests;
        this.filteredGuests = guests;
      },
      error: (err) => console.error('Error loading guests:', err)
    });
  }

  calculateTotal(): void {
    const roomId = this.bookingForm.get('roomId')?.value;
    const startDate = this.bookingForm.get('startDate')?.value;
    const endDate = this.bookingForm.get('endDate')?.value;
    
    if (roomId && startDate && endDate) {
      const room = this.rooms.find(r => r.id === roomId);
      if (room) {
        const start = new Date(startDate);
        const end = new Date(endDate);
        this.nights = Math.ceil((end.getTime() - start.getTime()) / (1000 * 3600 * 24));
        this.totalPrice = room.price * this.nights;
        this.bookingForm.patchValue({ total: this.totalPrice });
      }
    }
  }

  addGuest(guest: Guest): void {
    if (!this.selectedGuests.find(g => g.id === guest.id)) {
      this.selectedGuests.push(guest);
    }
    this.bookingForm.get('guestSearch')?.setValue('');
  }

  removeGuest(guestId: number): void {
    this.selectedGuests = this.selectedGuests.filter(g => g.id !== guestId);
  }

  onSubmit(): void {
    if (this.bookingForm.valid && this.selectedGuests.length > 0) {
      const booking: any = {
        roomId: this.bookingForm.get('roomId')?.value,
        startDate: this.formatDate(this.bookingForm.get('startDate')?.value),
        endDate: this.formatDate(this.bookingForm.get('endDate')?.value),
        guestIds: this.selectedGuests.map(g => g.id),
        total: this.totalPrice
      };
      
      this.bookingService.createBooking(booking).subscribe({
        next: (response) => {
          alert('¡Reserva creada exitosamente!');
          this.router.navigate(['/bookings']);
        },
        error: (err) => {
          console.error('Error creating booking:', err);
          alert('Error al crear la reserva: ' + err.error);
        }
      });
    }
  }

  formatDate(date: Date): string {
    return date.toISOString().split('T')[0];
  }
}