import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { RoomService, Room } from './../services/room/room.service';
import { BookingService } from './../services/booking/booking.service';
import { NgFor } from '@angular/common';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  imports: [NgFor],
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  roomTypes: any[] = [];
  rooms: Room[] = [];
  stats = {
    totalRooms: 0,
    availableRooms: 0,
    occupiedRooms: 0,
    roomTypes: 0
  };

  constructor(
    private router: Router,
    private roomService: RoomService,
    private bookingService: BookingService
  ) {}

  ngOnInit(): void {
    this.loadRooms();
    this.loadRoomTypes();
  }

  loadRooms(): void {
    this.roomService.getAllRooms().subscribe({
      next: (rooms) => {
        this.rooms = rooms;
        this.stats.totalRooms = rooms.length;
        this.stats.availableRooms = rooms.filter(r => !r.occupied).length;
        this.stats.occupiedRooms = rooms.filter(r => r.occupied).length;
      },
      error: (err) => console.error('Error loading rooms:', err)
    });
  }

  loadRoomTypes(): void {
    // Obtener tipos únicos de habitaciones desde la API
    this.roomService.getAllRooms().subscribe({
      next: (rooms) => {
        const uniqueTypes = [...new Map(rooms.map(room => 
          [room.roomType, { 
            name: room.roomType, 
            capacity: room.capacity,
            price: room.price,
            icon: this.getIconForRoomType(room.roomType)
          }]
        )).values()];
        
        this.roomTypes = uniqueTypes.map(type => ({
          name: type.name,
          icon: type.icon,
          description: this.getDescriptionForRoomType(type.name),
          capacity: type.capacity,
          price: type.price
        }));
        
        this.stats.roomTypes = this.roomTypes.length;
      },
      error: (err) => console.error('Error loading room types:', err)
    });
  }

  getIconForRoomType(type: string): string {
    const icons: { [key: string]: string } = {
      'Simple': '🛏️',
      'Suite': '👑',
      'Doble Matrimonial': '💑',
      'Doble Individual': '👥',
      'Grupal 3': '👨‍👩‍👦',
      'Grupal 4': '👨‍👩‍👧‍👦'
    };
    return icons[type] || '🏠';
  }

  getDescriptionForRoomType(type: string): string {
    const descriptions: { [key: string]: string } = {
      'Simple': 'Perfecta para viajeros solitarios, cómoda y económica.',
      'Suite': 'Lujo y confort, con vista panorámica y jacuzzi.',
      'Doble Matrimonial': 'Ideal para parejas, cama matrimonial y ambiente acogedor.',
      'Doble Individual': 'Dos camas individuales, perfecta para amigos o familia.',
      'Grupal 3': 'Espaciosa para grupos pequeños de 3 personas.',
      'Grupal 4': 'Amplia y cómoda, ideal para familias o grupos.'
    };
    return descriptions[type] || 'Habitación confortable para tu estadía.';
  }

  navigateToBooking(): void {
    this.router.navigate(['/booking/create']);
  }

  navigateToRooms(): void {
    this.router.navigate(['/rooms']);
  }

  selectRoomType(roomType: string): void {
    this.router.navigate(['/booking/create'], { queryParams: { type: roomType } });
  }
}