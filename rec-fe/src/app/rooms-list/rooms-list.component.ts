import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { RoomService, Room } from './../services/room/room.service';

@Component({
  selector: 'app-rooms-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="rooms-container">
      <div class="header">
        <h1>Nuestras Habitaciones</h1>
        <div class="filters">
          <select (change)="filterByType($event)">
            <option value="">Todos los tipos</option>
            <option *ngFor="let type of roomTypes" [value]="type">
              {{ type }}
            </option>
          </select>
        </div>
      </div>

      <div class="rooms-grid">
        <div class="room-card" *ngFor="let room of filteredRooms">
          <div class="room-status" [class.occupied]="room.occupied">
            {{ room.occupied ? 'Ocupado' : 'Disponible' }}
          </div>
          <div class="room-number">Habitación {{ room.roomNumber }}</div>
          <div class="room-type">{{ room.roomType }}</div>
          <div class="room-details">
            <div class="detail">
              <span>👤 Capacidad:</span>
              <strong>{{ room.capacity }} personas</strong>
            </div>
            <div class="detail">
              <span>💰 Precio:</span>
              <strong>Bs. {{ room.price }}/noche</strong>
            </div>
          </div>
          <button 
            class="btn-book" 
            [disabled]="room.occupied"
            (click)="bookRoom(room.id)">
            {{ room.occupied ? 'No disponible' : 'Reservar ahora' }}
          </button>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .rooms-container {
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

    .filters select {
      padding: 0.5rem 1rem;
      border-radius: 8px;
      border: 1px solid #ddd;
      font-size: 1rem;
    }

    .rooms-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
      gap: 2rem;
    }

    .room-card {
      background: white;
      border-radius: 12px;
      padding: 1.5rem;
      box-shadow: 0 2px 8px rgba(0,0,0,0.1);
      transition: all 0.3s ease;
      position: relative;
    }

    .room-card:hover {
      transform: translateY(-4px);
      box-shadow: 0 4px 12px rgba(0,0,0,0.15);
    }

    .room-status {
      position: absolute;
      top: 1rem;
      right: 1rem;
      padding: 0.25rem 0.75rem;
      border-radius: 20px;
      font-size: 0.75rem;
      font-weight: 600;
      background: #4caf50;
      color: white;
    }

    .room-status.occupied {
      background: #f44336;
    }

    .room-number {
      font-size: 1.5rem;
      font-weight: bold;
      color: #1a1a2e;
      margin-bottom: 0.5rem;
    }

    .room-type {
      color: #ff6b35;
      font-weight: 600;
      margin-bottom: 1rem;
    }

    .room-details {
      margin: 1rem 0;
      padding: 1rem 0;
      border-top: 1px solid #eee;
      border-bottom: 1px solid #eee;
    }

    .detail {
      display: flex;
      justify-content: space-between;
      margin-bottom: 0.5rem;
    }

    .btn-book {
      width: 100%;
      padding: 0.75rem;
      background: #ff6b35;
      color: white;
      border: none;
      border-radius: 8px;
      cursor: pointer;
      font-weight: 600;
      transition: all 0.3s ease;
    }

    .btn-book:hover:not(:disabled) {
      background: #e55a2a;
      transform: translateY(-2px);
    }

    .btn-book:disabled {
      background: #ccc;
      cursor: not-allowed;
    }
  `]
})
export class RoomsListComponent implements OnInit {
  rooms: Room[] = [];
  filteredRooms: Room[] = [];
  roomTypes: string[] = [];

  constructor(private roomService: RoomService) {}

  ngOnInit(): void {
    this.loadRooms();
  }

  loadRooms(): void {
    this.roomService.getAllRooms().subscribe({
      next: (data) => {
        this.rooms = data;
        this.filteredRooms = data;
        this.roomTypes = [...new Set(data.map(r => r.roomType))];
      },
      error: (err) => console.error('Error loading rooms:', err)
    });
  }

  filterByType(event: any): void {
    const type = event.target.value;
    this.filteredRooms = type ? this.rooms.filter(r => r.roomType === type) : this.rooms;
  }

  bookRoom(roomId: number): void {
    // Navegar a crear reserva con la habitación seleccionada
    import('@angular/router').then(router => {
      router.RouterModule;
    });
  }
}