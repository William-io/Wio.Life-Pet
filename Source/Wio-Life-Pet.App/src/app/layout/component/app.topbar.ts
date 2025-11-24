import { Component } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { StyleClassModule } from 'primeng/styleclass';
import { AppConfigurator } from './app.configurator';
import { LayoutService } from '../service/layout.service';
import { Auth } from '../../settings/service/auth';

@Component({
    selector: 'app-topbar',
    standalone: true,
    imports: [RouterModule, CommonModule, StyleClassModule, AppConfigurator],
    template: ` <div class="layout-topbar">
        <div class="layout-topbar-logo-container">
            <button class="layout-menu-button layout-topbar-action" (click)="layoutService.onMenuToggle()">
                <i class="pi pi-bars"></i>
            </button>
            <a class="layout-topbar-logo" routerLink="/">
                <svg viewBox="0 0 60 50" fill="none" xmlns="http://www.w3.org/2000/svg">
                    <!-- Almofada principal da pata -->
                    <ellipse cx="30" cy="30" rx="12" ry="10" fill="var(--primary-color)" />
                    
                    <!-- Dedos da pata (4 almofadas menores) -->
                    <ellipse cx="20" cy="18" rx="4" ry="6" fill="var(--primary-color)" transform="rotate(-15 20 18)" />
                    <ellipse cx="28" cy="15" rx="4" ry="6" fill="var(--primary-color)" />
                    <ellipse cx="36" cy="15" rx="4" ry="6" fill="var(--primary-color)" />
                    <ellipse cx="44" cy="18" rx="4" ry="6" fill="var(--primary-color)" transform="rotate(15 44 18)" />
                    
                    <!-- Detalhes da almofada principal (textura) -->
                    <ellipse cx="30" cy="28" rx="8" ry="6" fill="rgba(255,255,255,0.1)" />
                    
                    <!-- Pequenos detalhes nos dedos -->
                    <circle cx="20" cy="16" r="1.5" fill="rgba(255,255,255,0.2)" />
                    <circle cx="28" cy="13" r="1.5" fill="rgba(255,255,255,0.2)" />
                    <circle cx="36" cy="13" r="1.5" fill="rgba(255,255,255,0.2)" />
                    <circle cx="44" cy="16" r="1.5" fill="rgba(255,255,255,0.2)" />
                </svg>
                <span>WIO.LIFE PET</span>
            </a>
        </div>

        <div class="layout-topbar-actions">
                    <p class="layout-topbar-menu-content" style="margin-top: 6px; font-weight: bold;">Bem-vindo: <span style="color: green;">{{ username }}</span></p>
            <div class="layout-config-menu">
                <button type="button" class="layout-topbar-action" (click)="toggleDarkMode()">
                    <i [ngClass]="{ 'pi ': true, 'pi-moon': layoutService.isDarkTheme(), 'pi-sun': !layoutService.isDarkTheme() }"></i>
                </button>
                <div class="relative">
                    <button
                        class="layout-topbar-action layout-topbar-action-highlight"
                        pStyleClass="@next"
                        enterFromClass="hidden"
                        enterActiveClass="animate-scalein"
                        leaveToClass="hidden"
                        leaveActiveClass="animate-fadeout"
                        [hideOnOutsideClick]="true"
                    >
                        <i class="pi pi-palette"></i>
                    </button>
                    <app-configurator />
                </div>
            </div>

            <button class="layout-topbar-menu-button layout-topbar-action" pStyleClass="@next" enterFromClass="hidden" enterActiveClass="animate-scalein" leaveToClass="hidden" leaveActiveClass="animate-fadeout" [hideOnOutsideClick]="true">
                <i class="pi pi-ellipsis-v"></i>
            </button>

            <div class="layout-topbar-menu hidden lg:block">
                <div class="layout-topbar-menu-content">
                    <button type="button" class="layout-topbar-action">
                        <i class="pi pi-calendar"></i>
                        <span>Calendar</span>
                    </button>
                    <button type="button" class="layout-topbar-action">
                        <i class="pi pi-inbox"></i>
                        <span>Messages</span>
                    </button>
                    <button type="button" class="layout-topbar-action">
                        <i class="pi pi-user"></i>
                        <span>Profile</span>
                    </button>
                    <button type="button" class="layout-topbar-action" (click)="auth.logout()">
                        <i class="pi pi-power-off"></i>
                        <span>Exit</span>
                    </button>
                </div>
            </div>
        </div>
    </div>`
})
export class AppTopbar {
    items!: MenuItem[];

    username: string = JSON.parse(localStorage.getItem('user_vet')!).username;

    constructor(public layoutService: LayoutService, public auth: Auth) {}

    toggleDarkMode() {
        this.layoutService.layoutConfig.update((state) => ({ ...state, darkTheme: !state.darkTheme }));
    }
}
