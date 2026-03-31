import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from './../../../enviroment/enviroment';

export interface BookingRequest {
  roomId: number;
  startDate: string;
  endDate: string;
  guestIds: number[];
  total: number;
}

export interface BookingResponse {
  id: number;
  roomNumber: string;
  roomTypeName: string;
  startDate: string;
  endDate: string;
  status: string;
  total: number;
}

export interface Room {
  id: number;
  roomNumber: string;
  roomType: string;
  price: number;
  capacity: number;
  occupied: boolean;
}

export interface Guest {
  id: number;
  firstName: string;
  secondName: string;
  lastName: string;
  idCard: string;
  phoneNumber: string;
  email: string;
}

@Injectable({
  providedIn: 'root'
})
export class BookingService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getRooms(): Observable<Room[]> {
    return this.http.get<Room[]>(`${this.apiUrl}/Room/All`);
  }

  getGuests(): Observable<Guest[]> {
    return this.http.get<Guest[]>(`${this.apiUrl}/Guest/All`);
  }

  createBooking(booking: BookingRequest): Observable<BookingResponse> {
    return this.http.post<BookingResponse>(`${this.apiUrl}/Booking/Create`, booking);
  }

  getBookings(): Observable<BookingResponse[]> {
    return this.http.get<BookingResponse[]>(`${this.apiUrl}/Booking/All`);
  }

  checkIn(bookingId: number): Observable<BookingResponse> {
    return this.http.put<BookingResponse>(`${this.apiUrl}/Booking/checkin/${bookingId}`, {});
  }

  checkOut(bookingId: number): Observable<BookingResponse> {
    return this.http.put<BookingResponse>(`${this.apiUrl}/Booking/checkout/${bookingId}`, {});
  }

  cancelBooking(bookingId: number): Observable<BookingResponse> {
    return this.http.put<BookingResponse>(`${this.apiUrl}/Booking/cancel/${bookingId}`, {});
  }
}