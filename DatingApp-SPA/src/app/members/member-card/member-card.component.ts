import { Component, OnInit, Input } from '@angular/core';
import { User } from '../../_models/user';
import { AuthService } from '../../_services/auth.service';
import { UserService } from '../../_services/user.service';
import { AlertifyService } from '../../_services/alertify.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {
  @Input() user: User;

  constructor(private authService: AuthService, private userService: UserService, private alertify: AlertifyService) { }

  ngOnInit() {
  }

  sendLike(userId: number) {
    this.userService.sendLike(this.authService.decodedToken.nameid, userId).subscribe(data => {
      if (data != null) {
        this.alertify.success('You have liked ' + this.user.knownAs);
      } else {
        this.alertify.error('You have disliked ' + this.user.knownAs);
      }
    }, error => {
      this.alertify.error(error);
    });
  }
}
