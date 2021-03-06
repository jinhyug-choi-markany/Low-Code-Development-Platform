import React, { Component } from 'react';
import { Button } from 'reactstrap';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { Formik, Field, Form } from 'formik';
import { Redirect, withRouter } from 'react-router-dom';
import EyeIcon from 'mdi-react/EyeIcon';
import LoadingIcon from 'mdi-react/LoadingIcon';
import { login, logout, getCurrentUser, changePassword, ShowSpinner, getWebAuthnRegistrationRequest, webauthnRegistration } from '../../redux/Auth';
import { Row, Col } from 'reactstrap';
import NaviBar from '../../components/custom/NaviBar';
import { getNaviBar } from './index';
import ProgressBar from '../../components/custom/ProgressBar';
import queryString from 'query-string'
import Alert from '../../components/Alert';
import { Link } from 'react-router-dom';
import log from '../../helpers/logger';
import DocumentTitle from 'react-document-title';
import { setTitle } from '../../redux/Global';
import { parsedUrl, base64UrlEncode, base64UrlDecode, base64Codec, coerceToArrayBuffer, IsMobile } from '../../helpers/domutils';

class NewPassword extends Component {
  constructor(props) {
    super(props);
    this.titleSet = false;
    this.SystemName = (document.Rintagi || {}).systemName || 'Rintagi';
    this.state = {
      submitting: false,
      showPassword: false,
      j: '',
      p: ''
    };

    this.showPassword = this.showPassword.bind(this);
    this.changePassword = this.changePassword.bind(this);
    this.webauthnRegistration = this.webauthnRegistration.bind(this);
  }

  componentDidMount() {
    const qs = queryString.parse(this.props.location.search);
    const fido2 = typeof window != 'undefined' && window.PublicKeyCredential;
    if (fido2) {
      const _this = this;
      const myUrl = parsedUrl(window.location);
      this.props.getWebAuthnRegistrationRequest(myUrl.href)
              .then(result=>{
                log.debug(result);
                _this.setState({
                  registrationRequest: ((result || {}).data || {}).registrationRequest,
                });
              })
              .catch(error=>{
                log.debug(error);
              })
    }
    this.setState({
      j: qs.j,
      p: qs.p,
    });

  }
  componentDidUpdate(prevprops, prevstates) {
    const emptyTitle = '';
    const siteTitle = this.SystemName;

    if (!this.titleSet) {
      this.props.setTitle(siteTitle);
      this.titleSet = true;
    }
  }

  // componentDidMount() {
  //   if (!this.props.user || !this.props.user.UsrId) {
  //     this.props.getCurrentUser();
  //   }
  //   // console.log(this.props.user);
  // }
  // componentDidUpdate(prevProps, prevStats) {

  //   const from = ((this.props.location || {}).state || {}).from;
  //   if ((!prevProps.user || !prevProps.user.UsrId) && this.props.user && this.props.user.UsrId) {
  //     //window.location.reload();
  //   }

  //   if (this.state.submitting &&
  //     (((this.props.user || {}).status === "failed") ||
  //       ((this.props.user || {}).errMsg === "Failed to fetch")
  //     )
  //   ) {
  //     this.state.setSubmitting(false);
  //   }
  // }
  showPassword(e) {
    e.preventDefault();
    this.setState({
      showPassword: !this.state.showPassword,
      submitting: false,
    })
  }

  static getDerivedStateFromProps(nextProps, prevState) {
    if (prevState.submitting) {
      prevState.setSubmitting(false);
    }
    return null;
  }

  changePassword(values, { setSubmitting, setErrors, resetForm, setValues /* setValues and other goodies */ }) {
    this.setState({ submitting: true, setSubmitting: setSubmitting });
    console.log(values);

    this.props.changePassword(
      {
        j: this.state.j || '',
        p: this.state.p || '',
        newUsrPassword: values.cNewUsrPassword,
        confirmPwd: values.cConfirmPwd,
      }
    );
  }

  transformWebAuthRequest(request) {
    request.status = undefined;
    request.errorMessage = undefined;
    request.challenge = coerceToArrayBuffer(request.challenge, 'challenge');
    if (request.user && request.user.id) {
        request.user.id = coerceToArrayBuffer(request.user.id, 'user.id');
    }
    if (request.excludeCredentials && request.excludeCredentials) {
        request.excludeCredentials = (request.excludeCredentials || []).map(function (c, i) {
            c.id = coerceToArrayBuffer(c.id, 'excludeCredentials.' + i + '.id');
            return c;
        });
    }
    if (request.allowCredentials && request.allowCredentials) {
        request.allowCredentials = (request.allowCredentials || []).map(function (c, i) {
            c.id = coerceToArrayBuffer(c.id, 'allowCredentials.' + i + '.id');
            return c;
        });
    }
    if (request.authenticatorSelection && !request.authenticatorSelection.authenticatorAttachment) {
        request.authenticatorSelection.authenticatorAttachment = undefined;
    }
    return request;
  }

  getRegistrationRequestAsync() {
    const _this = this;
    const request = this.state.registrationRequest;
    return new Promise(function (resolve, reject) {
        if (!request) reject("no request json");
        else {
            try {
                const x = JSON.parse(request);
                resolve(request);
            } catch (e) {
                reject(e);
            }
        }
    });
  }
  
  webauthnRegistration(values, { setSubmitting, setErrors, resetForm, setValues /* setValues and other goodies */ }) {
    const _this = this;
    const b64url = new base64Codec(true);
    let registrationRequest = null;
    return function(evt) {
      _this.setState({ submitting: true, setSubmitting: setSubmitting });
      _this.getRegistrationRequestAsync()
          .then((request) => {
            registrationRequest = request;
            const x = JSON.parse(request);
            request = _this.transformWebAuthRequest(x);
              return navigator.credentials.create({
                  publicKey: request
              })
          })
          .then(function (newCredentialInfo) {
              var extensions = newCredentialInfo.getClientExtensionResults();
              var result = {
                  Id: newCredentialInfo.id,
                  RawId: b64url.encode(newCredentialInfo.rawId),
                  Type: newCredentialInfo.type,
                  Response: {
                      AttestationObject: b64url.encode(newCredentialInfo.response.attestationObject),
                      ClientDataJson: b64url.encode(newCredentialInfo.response.clientDataJSON),
                  },
                  Extensions: extensions,
                  attestationObject: b64url.encode(newCredentialInfo.response.attestationObject),
                  platform: navigator.platform,
                  isMobile: IsMobile(),
              };
              var resultJSON = JSON.stringify(result);
              _this.props.webauthnRegistration(registrationRequest, resultJSON)
              .then(result=>{
                log.debug(result);
                if (((result || {}).data || {}).credentialId) {
                  localStorage["Fido2Login"] = newCredentialInfo.id;
                  if (result.data.message) {
                    alert(result.data.message);
                  }
                }
              })
              .catch(error=>{
                log.debug(error);
                alert("registration failed");
              });
      }).catch((err) => {
          alert(err);
          // No acceptable authenticator or user refused consent. Handle appropriately.
      }).finally(
        ()=>{
          _this.setState({ submitting: false });  
        }
      );
      console.log(values);
  
    }
  }  

  render() {

    const naviBar = getNaviBar("NewPassword", this.props.auth.Label);
    const siteTitle = (this.props.global || {}).pageTitle || '';
    const emptyTitle = '';
    return (
      <DocumentTitle title={siteTitle}>
        <div>
          <div className='account'>
            <div className='account__wrapper'>
              <div className='account__card shadow-box rad-4'>
                {this.props.user.loading && <div className='panel__refresh'><LoadingIcon /></div>}
                {ShowSpinner(this.props.auth) && <div className='panel__refresh'><LoadingIcon /></div>}
                <div className='tabs tabs--justify tabs--bordered-bottom mb-30'>
                  <div className='tabs__wrap'>
                    <NaviBar history={this.props.history} navi={naviBar} />
                  </div>
                </div>
                <p className="project-title-mobile mb-10">{emptyTitle}</p>
                <div className='account__head'>
                  <h3 className='account__title'>{this.props.auth.Label.NewPassword}</h3>
                  <h4 className='account__subhead subhead'>{this.props.auth.Label.NewPasswordSubtitle}</h4>
                  {/* <h4>{(this.props.user || {}).UsrId}</h4> */}
                </div>
                <Formik
                  initialValues={{
                    cNewUsrPassword: '',
                    cConfirmPwd: '',
                    // password: '',
                  }}
                  validate={values => {
                    // // same as above, but feel free to move this into a class method now.
                    let errors = {};
                    if (!values.cNewUsrPassword) {
                      errors.cNewUsrPassword = this.props.auth.Label.NewUsrPasswordEmpty;
                    }
                    if (values.cNewUsrPassword !== values.cConfirmPwd) {
                      errors.cConfirmPwd = this.props.auth.Label.ConfirmPwdEmpty;
                    }
                    // if (!values.email) {
                    // //   errors.email = 'This field is required';
                    // // } else
                    // if (!/^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$/i.test(values.email)) {
                    //   errors.email = 'Invalid email address';
                    // }
                    return errors;
                  }}
                  onSubmit={this.changePassword}
                  render={({
                    errors,
                    touched,
                    setSubmitting,
                    isSubmitting,
                    values
                  }) => (
                      <Form className='form'> {/* this line equals to <form className='form' onSubmit={handleSubmit} */}
                        <div className='form__form-group'>
                          <div className='form__form-group-field'>
                            <ProgressBar
                              name="cPasswordStrength"
                              text={values.cNewUsrPassword}
                            />
                          </div>
                        </div>

                        <div className='form__form-group'>
                          <label className='form__form-group-label'>{this.props.auth.Label.NewuserPassword}</label>
                          <div className='form__form-group-field'>
                            <Field
                              type={this.state.showPassword ? 'text' : 'password'}
                              name="cNewUsrPassword"

                            />
                            <button className={`form__form-group-button${this.state.showPassword ? ' active' : ''}`}
                              onClick={(e) => this.showPassword(e)} type="button"><EyeIcon /></button>
                          </div>
                          {errors.cNewUsrPassword && touched.cNewUsrPassword && <span className='form__form-group-error'>{errors.cNewUsrPassword}</span>}

                        </div>
                        {/* <div className='form__form-group'>
                        <label className='form__form-group-label'>or</label>
                      </div> */}

                        <div className='form__form-group'>
                          <label className='form__form-group-label'>{this.props.auth.Label.ConfirmPwd}</label>
                          <div className='form__form-group-field'>
                            <Field
                              type={this.state.showPassword ? 'text' : 'password'}
                              name="cConfirmPwd"
                            />
                            <button className={`form__form-group-button${this.state.showPassword ? ' active' : ''}`}
                              onClick={(e) => this.showPassword(e)} type="button"><EyeIcon /></button>
                          </div>
                          {errors.cConfirmPwd && touched.cConfirmPwd && <span className='form__form-group-error'>{errors.cConfirmPwd}</span>}
                        </div>

                        <div className='form__form-group'>
                          <Alert color='info' className='alert--neutral' icon>
                            <p><span className='bold-text'>{this.props.auth.Label.Information}:</span> {this.props.auth.Label.PwdHlpMsgLabel}</p>
                          </Alert>
                        </div>

                        <div className="form__form-group mb-0">
                          <Row className="btn-bottom-row">
                            <Col xs={3} sm={2} className="btn-bottom-column">
                              <Button color='success' className='btn btn-outline-success account__btn' onClick={this.props.history.goBack} outline><i className="fa fa-long-arrow-left"></i></Button>
                            </Col>
                            <Col xs={9} sm={10} className="btn-bottom-column">
                              <Button color='success' className='btn btn-success account__btn' type="submit" disabled={isSubmitting}>{this.props.auth.Label.pdPwdBtn}</Button>
                            </Col>
                          </Row>
                        </div>
                        { this.state.registrationRequest &&
                          <div className="form__form-group mb-0">
                          <Row className="btn-bottom-row">
                            <Col xs={1} sm={12} className="btn-bottom-column">
                              <Button color='success' className='btn btn-success account__btn' onClick={this.webauthnRegistration(values, {setSubmitting})} disabled={isSubmitting}>{"WebAuthn Registration"}</Button>
                            </Col>
                          </Row>
                          </div>
                        }
                      </Form>
                    )}
                />
              </div>
            </div>
          </div>
        </div>
      </DocumentTitle>
    )
  }
}

const mapStateToProps = (state) => ({
  user: (state.auth || {}).user,
  auth: state.auth,
  global: state.global,
});

const mapDispatchToProps = (dispatch) => (
  bindActionCreators(Object.assign({},
    { login: login },
    { logout: logout },
    { getCurrentUser: getCurrentUser },
    { changePassword: changePassword },
    { getWebAuthnRegistrationRequest: getWebAuthnRegistrationRequest },
    { webauthnRegistration: webauthnRegistration },
    { setTitle: setTitle },
  ), dispatch)
)

export default withRouter(connect(mapStateToProps, mapDispatchToProps)(NewPassword));
